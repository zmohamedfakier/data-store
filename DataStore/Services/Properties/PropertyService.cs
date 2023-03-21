using DataStore.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Properties
{
  public class PropertyService : IPropertyService
  {
    private IEntityService _entityService;
    private Dictionary<string, PropertySet> _propertySet;
    private Dictionary<int, DescriptorPropertySet> _descriptorPropertySetByID;
    private IConfiguration _configuration;

    public PropertyService(IEntityService entityService, IConfiguration configuration)
    {
      _entityService = entityService;
      _propertySet = new Dictionary<string, PropertySet>(StringComparer.OrdinalIgnoreCase);
      _descriptorPropertySetByID = new Dictionary<int, DescriptorPropertySet>();
      _configuration = configuration;
    }

    public void Load()
    {
      Dictionary<int, PropertySet> quickLookup = new Dictionary<int, PropertySet>();

      using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DataStore")))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = "[Meta].[LoadProperties]";

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int id = reader.GetFieldValue<int>(0);
              string name = reader.GetFieldValue<string>(1);
              int typeID = reader.GetFieldValue<int>(2);
              int? codeSetID = reader.IsDBNull(3) ? null : reader.GetFieldValue<int>(3);

              PropertySet set = new PropertySet(id, name, (PropertySetType)typeID, codeSetID);
              _propertySet.Add(name, set);
              quickLookup.Add(id, set);
            }

            reader.NextResult();
            int previousId = 0;
            PropertySet valuesSet = null;

            while (reader.Read())
            {
              int id = reader.GetFieldValue<int>(0);
              int propertySetId = reader.GetFieldValue<int>(1);
              string value = reader.IsDBNull(2) ? null : reader.GetFieldValue<string>(2);
              int? entityId = reader.IsDBNull(3) ? null : reader.GetFieldValue<int>(3);

              if (id != previousId)
              {
                if (!quickLookup.TryGetValue(propertySetId, out valuesSet))
                  throw new Exception($"There is no property set with the id {id}.");

                previousId = id;
              }

              valuesSet.Add(new PropertyValue() { ID = id, Value = value, EntityID = entityId });
            }
          }
        }
      }
    }

    public DescriptorPropertySet GetPropertySet(int id)
    {
      string connectionString = _configuration.GetConnectionString("DataStore");
      List<DescriptorPropertyValue> properties = new List<DescriptorPropertyValue>();

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = "[Meta].[GetDescriptorPropertySet]";
          sqlCommand.CommandType = CommandType.StoredProcedure;
          sqlCommand.Parameters.Add(new SqlParameter("@SetID", SqlDbType.Int)).Value = id;

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {

              int propertySetID = reader.GetFieldValue<int>(0);
              string propertySet = reader.GetFieldValue<string>(1);
              int valueID = reader.GetFieldValue<int>(2);
              int entityID = reader.GetFieldValue<int>(3);
              string value = reader.GetFieldValue<string>(4);

              DescriptorPropertyValue dpv = new DescriptorPropertyValue();
              dpv.PropertySetID = propertySetID;
              dpv.PropertySet = propertySet;
              dpv.EntityID = entityID;
              dpv.ValueID = valueID;
              dpv.Value = value;

              properties.Add(dpv);
            }
          }
        }

        DescriptorPropertySet dps = new DescriptorPropertySet(properties);
        return dps;
      }
    }

    public DescriptorPropertySet GetPropertySet(IEnumerable<Tuple<string, string>> properties)
    {
      if (!properties.Any())
        return DescriptorPropertySet.Empty;

      List<DescriptorPropertyValue> descriptorPropertyValues = new List<DescriptorPropertyValue>();

      foreach (Tuple<string, string> pr in properties)
      {
        DescriptorPropertyValue dpv = new DescriptorPropertyValue();

        if (!_propertySet.TryGetValue(pr.Item1, out PropertySet matchedSet))
          throw new Exception($"The property set {pr.Item1} does not exist.");

        if (matchedSet.Type == PropertySetType.EntitySet)
        {
          if (matchedSet.CodeSetID.HasValue)
          {
            IEntityDescriptor descriptor = _entityService.GetEntity(pr.Item2, matchedSet.CodeSetID.Value);
            if (descriptor == null)
              throw new Exception($"The entity {pr.Item2} does not exist.");

            descriptorPropertyValues.Add(new DescriptorPropertyValue() { PropertySet = pr.Item1, Value = pr.Item2, PropertySetID = matchedSet.ID, EntityID = descriptor.ID, ValueID = 0 });
            continue;
          }
        }

        if (!matchedSet.TryGetValue(pr.Item2, out PropertyValue pv))
          throw new Exception($"The property set {pr.Item1} does not exist.");

        descriptorPropertyValues.Add(new DescriptorPropertyValue() { PropertySet = pr.Item1, Value = pr.Item2, PropertySetID = matchedSet.ID, EntityID = 0, ValueID = pv.ID });
      }

      DescriptorPropertySet dps = new DescriptorPropertySet(descriptorPropertyValues);

      return dps;
    }
  }
}
