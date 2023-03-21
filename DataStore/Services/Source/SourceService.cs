using DataStore.Api;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Source
{
  public class SourceService : ISourceService
  {
    private IServiceProvider _provider;

    public SourceService(IServiceProvider provider)
    {
      _provider = provider;
    }

    public ISourceDescriptor GetSource(string source)
    {
      if (source == null)
        return null;

      using (IServiceScope scope = _provider.CreateScope())
      {
        SourceDbContext dbContext = scope.ServiceProvider.GetService<SourceDbContext>();
        ISourceDescriptor sourceDescriptor = dbContext.Source.Where(s => s.Name == source).FirstOrDefault();
        if (sourceDescriptor == null)
          throw new Exception($"The source {source} is not a valid source.");

        return sourceDescriptor;
      }
    }

    public ISourceDescriptor GetSource(int source)
    {
      using (IServiceScope scope = _provider.CreateScope())
      {
        SourceDbContext dbContext = scope.ServiceProvider.GetService<SourceDbContext>();
        ISourceDescriptor sourceDescriptor = dbContext.Source.Where(s => s.ID == source).FirstOrDefault();
        if (sourceDescriptor == null)
          throw new Exception($"The source {source} is not a valid source.");

        return sourceDescriptor;
      }
    }
  }
}
