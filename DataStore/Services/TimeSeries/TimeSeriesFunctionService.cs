using DataStore.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.TimeSeries
{
  public class TimeSeriesFunctionService
  {
    private IFunctionFactoryService _functionFactory;
    private IConfiguration _configuration;
    private ISourceService _sourceService;
    private IPropertyService _propertyService;

    public TimeSeriesFunctionService(IFunctionFactoryService factory, IConfiguration configuration, ISourceService sourceService, IPropertyService propertyService)
    {
      _functionFactory = factory;
      _configuration = configuration;
      _sourceService = sourceService;
      _propertyService = propertyService;
    }

    public List<Tuple<TimeSeriesRequest, ITimeSeriesFunction>> GetFunction(IEntityDescriptor entity, IEntityDescriptor timeseriesEntity, DescriptorPropertySet propertyList, ISourceDescriptor source, bool match)
    {
      List<Tuple<TimeSeriesRequest, ITimeSeriesFunction>> matchedFunctions = new List<Tuple<TimeSeriesRequest, ITimeSeriesFunction>>();

      string connectionString = _configuration.GetConnectionString("DataStore");
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        using (SqlCommand sqlCommand = connection.CreateCommand())
        {
          DataTable dt = new DataTable();
          dt.Columns.Add("PropertySetID", typeof(int));
          dt.Columns.Add("ValueID", typeof(int));
          dt.Columns.Add("EntityID", typeof(int));

          foreach (DescriptorPropertyValue dpv in propertyList)
            dt.Rows.Add(dpv.PropertySetID, dpv.ValueID, dpv.EntityID);

          sqlCommand.CommandText = "[Meta].[MatchRequests]";
          sqlCommand.CommandType = CommandType.StoredProcedure;
          sqlCommand.Parameters.Add(new SqlParameter("@EntityID", SqlDbType.Int)).Value = entity.ID;
          sqlCommand.Parameters.Add(new SqlParameter("@TimeSeriesID", SqlDbType.Int)).Value = timeseriesEntity.ID;
          SqlParameter parameter = sqlCommand.Parameters.Add(new SqlParameter("@Properties", SqlDbType.Structured));
          parameter.Value = dt;
          parameter.TypeName = "[Meta].[PropertyValue]";
          sqlCommand.Parameters.Add(new SqlParameter("@SourceID", SqlDbType.Int)).Value = source == null ? DBNull.Value : source.ID;
          sqlCommand.Parameters.Add(new SqlParameter("@MatchPartialProperties", SqlDbType.Int)).Value = match ? 1 : 0;

          using (SqlDataReader reader = sqlCommand.ExecuteReader())
          {
            while (reader.Read())
            {
              int descriptorID = reader.GetFieldValue<int>(0);
              int sourceID = reader.GetFieldValue<int>(3);
              int propertySetID = reader.GetFieldValue<int>(4);
              int functionID = reader.GetFieldValue<int>(5);

              ISourceDescriptor resolvedSource = (source?.ID ?? 0) == sourceID ? source : _sourceService.GetSource(sourceID);
              DescriptorPropertySet resolvedPropertySet = propertySetID == 0 ? DescriptorPropertySet.Empty : _propertyService.GetPropertySet(propertySetID);

              TimeSeriesRequest matchedRequest = new TimeSeriesRequest() { Entity = entity, TimeSeries = timeseriesEntity, Source = resolvedSource, Properties = resolvedPropertySet, DescriptorID = descriptorID };
              ITimeSeriesFunction function = _functionFactory.GetFunction<ITimeSeriesFunction>(functionID);
              matchedFunctions.Add(new Tuple<TimeSeriesRequest, ITimeSeriesFunction>(matchedRequest, function));
            }
          }
        }
      }

      return matchedFunctions;
    }
  }
}
