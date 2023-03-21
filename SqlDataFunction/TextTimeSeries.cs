using DataStore.Api;
using DataStore.Api.Series;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Functions
{
  public class TextTimeSeriesFunction : BaseTimeSeriesFunction<TextTimeSeries>
  {
    public TextTimeSeriesFunction(IConfiguration configuration)
      : base(configuration, "[Data].[GetTextTimeSeries]")
    {
    }

    public override bool Equals(object obj)
    {
      return obj is TextTimeSeriesFunction;
    }

    public override int GetHashCode()
    {
      return "TextTimeSeriesFunction".GetHashCode();
    }
  }
}
