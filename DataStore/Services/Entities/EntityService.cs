using DataStore.Api;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using DataStore.Api.Entities;
using Microsoft.Extensions.Configuration;

namespace DataStore.Services.Entities
{
  public class EntityService : IEntityService
  {
    private Dictionary<int, IEntityDescriptorCollection> _entities;
    private Dictionary<int, CodeSet> _codeSets;
    private Dictionary<string, int> _codeSetLookup;
    private List<int> _codePreference;
    private IConfiguration _configuration;

    public EntityService(IConfiguration configuration)
    {
      _codeSets = new Dictionary<int, CodeSet>();
      _codeSetLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
      _codePreference = new List<int>();
      _entities = new Dictionary<int, IEntityDescriptorCollection>();
      _configuration = configuration;
    }


    public void LoadData()
    {
      LoadCodeSets();
      LoadCodePreference();
      LoadCodes();
    }

    private void LoadCodePreference()
    {
      using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DataStore")))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = "[Entity].[LoadCodePreference]";

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int codeSetID = reader.GetFieldValue<int>(0);
              _codePreference.Add(codeSetID);
            }
          }
        }
      }
    }

    private void LoadCodes()
    {
      using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DataStore")))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = "[Entity].[LoadCodes]";

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int entityID = reader.GetFieldValue<int>(0);
              string code = reader.GetFieldValue<string>(1);
              int codeSetId = reader.GetFieldValue<int>(2);

              if (!_entities.TryGetValue(entityID, out IEntityDescriptorCollection codeCollection))
              {
                codeCollection = new EntityDescriptorCollection(_codeSetLookup);
                _entities.Add(entityID, codeCollection);
              }

              if (!_codeSets.TryGetValue(codeSetId, out CodeSet codeSet))
                throw new Exception("The code set is not valid");

              IEntityDescriptor descriptor = new EntityDescriptor(entityID, code, codeSetId, codeSet.Name, codeCollection);
              codeCollection.Add(descriptor);

              codeSet.Add(descriptor);
            }
          }
        }
      }
    }

    private void LoadCodeSets()
    {
      string connectionString = _configuration.GetConnectionString("DataStore");
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = "[Entity].[LoadCodeSets]";

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int id = reader.GetFieldValue<int>(0);
              string name = reader.GetFieldValue<string>(1);

              CodeSet cs = new CodeSet(id, name);
              _codeSetLookup.Add(name, id);
              _codeSets.Add(id, cs);
            }
          }
        }
      }
    }

    public IEntityDescriptor GetEntity(string entity, int codeSetID)
    {
      string codeSet = "";
      foreach (KeyValuePair<string, int> pr in _codeSetLookup)
      {
        if (pr.Value == codeSetID)
          codeSet = pr.Key;
      }

      return LookupWithCodeType(entity, codeSet, codeSetID);
    }


    public IEntityDescriptor GetEntity(string entity)
    {
      int splitter = entity.LastIndexOf(':');
      if (splitter == -1 || splitter == entity.Length-1)
        return LookupWithCodePreference(entity);

      string code = entity.Substring(0, splitter);
      string codeSet = entity.Substring(splitter+1);
      
      if (_codeSetLookup.TryGetValue(codeSet, out int codeSetID))
        return LookupWithCodeType(code, codeSet, codeSetID);

      return LookupWithCodePreference(entity);
    }

    private IEntityDescriptor LookupWithCodePreference(string entityCode)
    {
      foreach (int codeSetID in _codePreference)
      {
        if (_codeSets.TryGetValue(codeSetID, out CodeSet codeSet))
        {
          if (codeSet.TryGetValue(entityCode, out IEntityDescriptor entityDescriptor))
          {
            return entityDescriptor;
          }
        }
      }

      return null;
    }

    private IEntityDescriptor LookupWithCodeType(string entityCode, string codeSet, int codeSetID)
    {
      if (!_codeSets.TryGetValue(codeSetID, out CodeSet set))
        return null;

      if (!set.TryGetValue(entityCode, out IEntityDescriptor entityDescriptor))
        return null;

      return entityDescriptor;
    }
  }
}
