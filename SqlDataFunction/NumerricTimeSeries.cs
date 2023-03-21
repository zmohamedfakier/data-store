using DataStore.Api;
using DataStore.Api.Series;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Functions
{
  public class NumericTimeSeriesFunction : BaseTimeSeriesFunction<NumericTimeSeries>
  {
    public NumericTimeSeriesFunction(IConfiguration configuration)
      : base(configuration, "[Data].[GetNumericTimeSeries]")
    {
    }

    public override bool Equals(object obj)
    {
      return obj is NumericTimeSeriesFunction;
    }

    public override int GetHashCode()
    {
      return "NumericTimeSeriesFunction".GetHashCode();
    }
  }
}
