using DataStore.Api;
using System;
using System.Collections.Generic;
using DataStore.Api.Series;

namespace DataStore.Services.TimeSeries
{
  public class TimeSeriesService : ITimeSeriesService
  {
    IEntityService _entityService;
    TimeSeriesFunctionService _functionService;
    ISourceService _sourceService;
    IPropertyService _propertyService;
    IDateSetService _dateService;

    public TimeSeriesService(IEntityService entityService, TimeSeriesFunctionService functionService, ISourceService sourceService, IPropertyService propertyService, IDateSetService dateService)
    {
      _entityService = entityService;
      _functionService = functionService;
      _sourceService = sourceService;
      _propertyService = propertyService;
      _dateService = dateService;
    }

    public TimeSeriesCollection Execute(IEnumerable<TimeSeriesRequest> requests, DateSet dates, Options options)
    {
      TimeSeriesRequestQueue queue = new TimeSeriesRequestQueue();
      foreach (TimeSeriesRequest request in requests)
      {
        List<Tuple<TimeSeriesRequest, ITimeSeriesFunction>> resolvedFunctions = _functionService.GetFunction(request.Entity, request.TimeSeries, request.Properties, request.Source, options.MatchPartialProperties);

        foreach (Tuple<TimeSeriesRequest, ITimeSeriesFunction> pr in resolvedFunctions)
        {
          queue.Queue(pr.Item1, pr.Item2);
        }
      }

      return queue.Process(dates, options.RollForward);
    }

    public TimeSeriesCollection Execute(TimeSeriesFunctionBody parameters)
    {
      TimeSeriesRequestQueue queue = new TimeSeriesRequestQueue();

      DateSet dateSet = _dateService.GetDateSet(parameters.Dates.Start, parameters.Dates.End, parameters.Dates.Interval);

      foreach (string entity in parameters.Entities)
      {
        IEntityDescriptor entityDescriptor = _entityService.GetEntity(entity);
        if (entityDescriptor == null)
          throw new Exception($"The entity code {entity} is not a valid entity");

        foreach (TimeSeriesFunctionRequest r in parameters.Metrics)
        {
          IEntityDescriptor timeSeriesDescriptor = _entityService.GetEntity(r.TimeSeries);
          if (timeSeriesDescriptor == null)
            throw new Exception($"The timeseries code {r.TimeSeries} is not a valid timeseries");

          DescriptorPropertySet propertySet = _propertyService.GetPropertySet(r.Properties);
          ISourceDescriptor source = _sourceService.GetSource(r.Source);

          List<Tuple<TimeSeriesRequest, ITimeSeriesFunction>> resolvedFunctions = _functionService.GetFunction(entityDescriptor, timeSeriesDescriptor, propertySet, source, parameters.Options.MatchPartialProperties);

          foreach (Tuple<TimeSeriesRequest, ITimeSeriesFunction> pr in resolvedFunctions)
          {
            queue.Queue(pr.Item1, pr.Item2);
          }
        };
      }

      return queue.Process(dateSet, parameters.Options.RollForward);
    }
  }
}
