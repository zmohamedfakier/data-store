using DataStore.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace DataStore.Services
{
  public class FunctionFactoryService : IFunctionFactoryService
  {
    private Dictionary<int, System.RuntimeTypeHandle> _functions;
    private Dictionary<string, Assembly> _assemblies;
    private string _binPath;

    private IServiceProvider _serviceProvider;
    private IConfiguration _configuration;

    public FunctionFactoryService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
      _functions = new Dictionary<int, RuntimeTypeHandle>();
      _assemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

      _serviceProvider = serviceProvider;
      _configuration = configuration;
    }

    public void LoadFunctions()
    {
      AssemblyLoadContext.Default.Resolving += Default_Resolving;

      _binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      string connectionString = _configuration.GetConnectionString("DataStore");
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = "[Functions].[LoadFunctions]";

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int functionID = reader.GetFieldValue<int>(0);
              string assemblyFile = reader.GetFieldValue<string>(1);
              string type = reader.GetFieldValue<string>(2);
              
              if (!_assemblies.TryGetValue(assemblyFile, out Assembly assembly))
              {
                assembly = LoadAssembly(assemblyFile, _binPath);
                _assemblies.Add(assemblyFile, assembly);
              }

              System.Type functionType = assembly.ExportedTypes.FirstOrDefault(t => t.FullName == type);
              if (functionType == null)
                throw new Exception($"The type {type} could not be found in the assembly {assembly.FullName}.");

              _functions.Add(functionID, functionType.TypeHandle);
            }
          }
        }
      }
    }

    private Assembly Default_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
    {
      string assemblyPath = Path.Combine(_binPath, arg2.Name) + ".dll";
      return arg1.LoadFromAssemblyPath(assemblyPath);
    }

    private Assembly LoadAssembly(string assemblyFile, string currentPath)
    {
      FileInfo assemblyPath = new FileInfo(Path.Combine(currentPath, assemblyFile));
      if (!assemblyPath.Exists)
        throw new Exception("The assembly does not exist");

      Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath.FullName);
      return assembly;
    }

    public T GetFunction<T>(int functionID) where T : class
    {
      if (!_functions.TryGetValue(functionID, out RuntimeTypeHandle rth))
        throw new Exception($"The function {functionID} does not exist.");

      System.Type type = System.Type.GetTypeFromHandle(rth);

      ConstructorInfo[] ci = type.GetConstructors(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      if (ci.Length != 1)
        throw new Exception($"The types required to initialise {type.FullName} cannot be resolved.");

      ParameterInfo[] parameters = ci[0].GetParameters();
      object[] arguments = new object[parameters.Length];
      int i = 0;

      foreach (ParameterInfo pi in parameters)
      {
        object resolvedDi = _serviceProvider.GetService(pi.ParameterType);
        if (resolvedDi == null)
          throw new Exception($"The types required to initialise {type.FullName} cannot be resolved.");

        arguments[i] = resolvedDi;
        ++i;
      }

      object function = ci[0].Invoke(arguments);

      if (function is not T requiredType)
        throw new Exception($"The function resolved to a {type.FullName} which does not implement {typeof(T).FullName}.");

      return requiredType;
    }
  }

  public class FunctionLoadContext : AssemblyLoadContext
  {
    private AssemblyDependencyResolver _resolver;

    public FunctionLoadContext(string location)
    {
      _resolver = new AssemblyDependencyResolver(location);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
      string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
      if (assemblyPath != null)
      {
        return LoadFromAssemblyPath(assemblyPath);
      }

      return null;
    }

  }
}
