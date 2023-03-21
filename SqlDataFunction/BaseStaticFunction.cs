using DataStore.Api;
using DataStore.Api.Series;
using DataStore.Api.Series.Static;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Functions
{
  public abstract class BaseStaticFunction<T> : ITimeSeriesFunction
    where T : IStaticSeriesValues, new()
  {
    private IConfiguration _configuration;
    private string _storedProcedure;

    public BaseStaticFunction(IConfiguration configuration, string storedProcedure)
    {
      _configuration = configuration;
      _storedProcedure = storedProcedure;
    }

    public TimeSeriesCollection Execute(IEnumerable<TimeSeriesRequest> requests, DateSet dateSet)
    {
      string connectionString = _configuration.GetConnectionString("DataStore");

      DataTable table = new DataTable();
      table.Columns.Add("DescriptorID", typeof(int));

      Dictionary<int, TimeSeriesImplementation> mappedRequest = new Dictionary<int, TimeSeriesImplementation>();
      Dictionary<int, T> values = new Dictionary<int, T>();
      TimeSeriesCollection collection = new TimeSeriesCollection();

      foreach (TimeSeriesRequest tsr in requests)
      {
        T ts = new T();
        TimeSeriesImplementation tsi = new TimeSeriesImplementation(tsr, ts);
        mappedRequest.Add(tsr.DescriptorID, tsi);
        values.Add(tsr.DescriptorID, ts);
        collection.Add(tsi);
        table.Rows.Add(tsr.DescriptorID);
      }

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = _storedProcedure;
          sqlCommand.CommandType = CommandType.StoredProcedure;
          SqlParameter parameter = sqlCommand.Parameters.AddWithValue("@Descriptors", table);
          parameter.SqlDbType = SqlDbType.Structured;
          parameter.TypeName = "[Data].[Descriptors]";


          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int descriptorID = reader.GetFieldValue<int>(0);
              object value = reader.GetValue(1);

              if (!values.TryGetValue(descriptorID, out T ts))
                continue;

              ts.SetValue(value);
            }
          }
        }
      }

      return collection;
    }
  }
}
