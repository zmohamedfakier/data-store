using DataStore.Api;
using DataStore.Services.Dates;
using DataStore.Services.Properties;
using DataStore.Services.Source;
using DataStore.Services.TimeSeries;
using DataStore.Services.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStore.Services;
using DataStore.Api.Series;

namespace DataStore
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<SourceDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DataStore")));

      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataStore", Version = "v1" });
      });

      services.AddAuthentication();
      services.AddSingleton<IPropertyService, PropertyService>();
      services.AddSingleton<IDateSetService, DateSetService>();
      services.AddSingleton<IEntityService>(new EntityService(Configuration));
      services.AddSingleton<ISourceService, SourceService>();
      services.AddSingleton<IFunctionFactoryService, FunctionFactoryService>();

      services.AddSingleton<TimeSeriesFunctionService>();
      services.AddSingleton<ITimeSeriesService, TimeSeriesService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataStore v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
