using DataStore.Api;
using DataStore.Api.Series;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
  public class ExchangeRateFunction : ITimeSeriesFunction
  {
    private IConfiguration _configuration;

    public ExchangeRateFunction(IConfiguration configurationService)
    {
      _configuration = configurationService;
    }

    public TimeSeriesCollection Execute(IEnumerable<TimeSeriesRequest> requests, DateSet dateSet)
    {
      string connectionString = _configuration.GetConnectionString("ExchangeRate");

      TimeSeriesCollection collection = new TimeSeriesCollection();

      TimeSeriesRequest request = requests.First();

      NumericTimeSeries exchangeRate = new NumericTimeSeries();
      TimeSeriesImplementation tsi = new TimeSeriesImplementation(request, exchangeRate);
      collection.Add(tsi);

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          sqlCommand.CommandText = "SELECT Date, Value FROM usd_exchange_rate WHERE Currency = 'ZAR'";

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              DateTime date = reader.GetFieldValue<DateTime>(0);
              double value = reader.GetFieldValue<double>(1);

              exchangeRate.Add(date, date, value);
            }
          }
        }
      }

      return collection;
    }
  }
}
