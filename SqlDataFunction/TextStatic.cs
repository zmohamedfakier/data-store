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
  public class TextStaticFunction : BaseStaticFunction<TextStatic>, ITimeSeriesFunction
  {
    public TextStaticFunction(IConfiguration configuration)
      : base(configuration, "[Data].[GetTextStatic]")
    {
    }

    public override bool Equals(object obj)
    {
      return obj is TextStaticFunction;
    }

    public override int GetHashCode()
    {
      return "TextStaticFunction".GetHashCode();
    }
  }
}
