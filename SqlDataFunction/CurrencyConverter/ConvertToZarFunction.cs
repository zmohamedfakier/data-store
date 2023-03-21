using DataStore.Api;
using DataStore.Api.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
  public class ConvertToZarFunction : ITimeSeriesFunction
  {
    ITimeSeriesService _timeSeriesService;
    IEntityService _entityService;

    public ConvertToZarFunction(ITimeSeriesService timeseriesService, IEntityService entityService)
    {
      _timeSeriesService = timeseriesService;
      _entityService = entityService;
    }

    public TimeSeriesCollection Execute(IEnumerable<TimeSeriesRequest> requests, DateSet dateSet)
    {
      List<TimeSeriesRequest> usdRequests = new List<TimeSeriesRequest>();

      IEntityDescriptor usdCurrency = _entityService.GetEntity("USD:ISO Currency Code");
      IEntityDescriptor zarCurrency = _entityService.GetEntity("ZAR:ISO Currency Code");

      foreach (TimeSeriesRequest original in requests)
      {
        DescriptorPropertySet replaceWithUSD = ReplaceCurrency(original.Properties, usdCurrency, "USD");
        TimeSeriesRequest usd = new TimeSeriesRequest() { Entity = original.Entity, TimeSeries = original.TimeSeries, Source = original.Source, Properties = replaceWithUSD };
        usdRequests.Add(usd);
      }

      DateSet ds = new DateSet(dateSet.Start, dateSet.End, Interval.Raw);
      Options options = new Options() { RollForward = false, MatchPartialProperties = true };

      TimeSeriesCollection usdValues = _timeSeriesService.Execute(usdRequests, ds, options);
      ISeriesValues exchangeRateValues = GetExchangeRateSeries(ds, options);

      TimeSeriesCollection convertedValues = new TimeSeriesCollection();
      foreach (ITimeSeries series in usdValues)
      {
        NumericTimeSeries convertedSeries = new NumericTimeSeries();
        foreach (IDataPoint point in series.Values)
        {
          IDataPoint exchanngeValue = exchangeRateValues.GetPoint(point.ValueDate);
          if (exchanngeValue == null)
            continue;

          double result = ((double)exchanngeValue.Data * (double)point.Data);
          convertedSeries.Add(point.ValueDate, point.DeclarationDate, result);
        }

        TimeSeriesImplementation tsi = new TimeSeriesImplementation(new TimeSeriesRequest() { Entity = series.Entity, TimeSeries = series.TimeSeries, Source = series.Source, Properties = ReplaceCurrency(series.Properties, zarCurrency, "ZAR") }, convertedSeries);
        convertedValues.Add(tsi);
      }

      return convertedValues;
    }

    private ISeriesValues GetExchangeRateSeries(DateSet dates, Options options)
    {
      IEntityDescriptor usdZar = _entityService.GetEntity("USDZAR");
      IEntityDescriptor timesereis = _entityService.GetEntity("Exchange Rate:TimeSeries");

      TimeSeriesRequest tsr = new TimeSeriesRequest() { Entity = usdZar, TimeSeries = timesereis, Properties = DescriptorPropertySet.Empty, Source = null };
      TimeSeriesRequest[] requests = new TimeSeriesRequest[] { tsr };
      TimeSeriesCollection result = _timeSeriesService.Execute(requests, dates, options);
      ITimeSeries forex = result.FirstOrDefault();
      return forex.Values;
    }

    private DescriptorPropertySet ReplaceCurrency(DescriptorPropertySet properties, IEntityDescriptor usdCurrency, string code)
    {
      List<DescriptorPropertyValue> values = new List<DescriptorPropertyValue>();
      foreach (DescriptorPropertyValue dpv in properties)
      {
        if (String.Equals(dpv.PropertySet, "currency", StringComparison.OrdinalIgnoreCase))
        {
          DescriptorPropertyValue usdValue = new DescriptorPropertyValue() { EntityID = usdCurrency.ID, PropertySet = dpv.PropertySet, PropertySetID = dpv.PropertySetID, Value = code, ValueID = 0 };
          values.Add(usdValue);
          continue;
        }

        values.Add(dpv);
      }

      return new DescriptorPropertySet(values);
    }
  }
}
