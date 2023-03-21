using DataStore.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using DataStore.Services;
using DataStore.Services.Entities;
using DataStore.Services.Properties;

namespace DataStore
{
  public class Program
  {
    public static void Main(string[] args)
    {
      IHost host = CreateHostBuilder(args).Build();

      FunctionFactoryService functionService = (FunctionFactoryService)host.Services.GetService<IFunctionFactoryService>();
      functionService.LoadFunctions();

      EntityService entityService = (EntityService)host.Services.GetService<IEntityService>();
      entityService.LoadData();

      PropertyService propertyService = (PropertyService)host.Services.GetService<IPropertyService>();
      propertyService.Load();

      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
