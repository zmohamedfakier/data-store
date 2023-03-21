using DataStore.Api;
using DataStore.Api.Series;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
  public abstract class BaseTimeSeriesFunction<T> : ITimeSeriesFunction 
    where T : ITimeSeriesValues, new()
  {
    private IConfiguration _configuration;
    private string _storedProcedure;

    public BaseTimeSeriesFunction(IConfiguration configurationService, string storedProcedure)
    {
      _configuration = configurationService;
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

          sqlCommand.Parameters.AddWithValue("@StartDate", dateSet.Start);
          sqlCommand.Parameters.AddWithValue("@EndDate", dateSet.End);

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int descriptorID = reader.GetFieldValue<int>(0);
              if (!values.TryGetValue(descriptorID, out T ts))
                continue;

              DateTime declarationDate = reader.GetFieldValue<DateTime>(1);
              DateTime valueDate = reader.GetFieldValue<DateTime>(2);
              object value = reader.GetValue(3);

              ts.Add(valueDate, declarationDate, value);
            }
          }
        }
      }

      return collection;
    }
  }
}
