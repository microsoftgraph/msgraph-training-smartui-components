using System;
using System.IO;
using GroupsReact.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GroupsReact.Extensions;

namespace GroupsReact
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

      if (env.IsDevelopment())
      {
        builder.AddUserSecrets<Startup>();
      }
      Configuration = builder.Build();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<AzureAdOptions>(options => Configuration.GetSection("AzureAd").Bind(options));

      services.AddAuthentication(sharedOptions =>
      {
        sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
      })
      .AddAzureAd()
      .AddCookie();

      services.AddMvc();

      // This sample uses an in-memory cache for tokens and subscriptions. Production apps will typically use some method of persistent storage.
      services.AddMemoryCache();

      // Add application services.
      services.AddSingleton(Configuration);
      services.AddSingleton<IGraphAuthProvider, GraphAuthProvider>();
      services.AddSingleton<MSALLogCallback>();
      services.AddTransient<IGraphSdkHelper, GraphSdkHelper>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IHostingEnvironment env, IApplicationBuilder app, IServiceProvider serviceProvider)
    {
      if (env.IsDevelopment())
      {
        var msalLogger = serviceProvider.GetService<MSALLogCallback>();
        Microsoft.Identity.Client.Logger.LogCallback = msalLogger.Log;

        app.UseDeveloperExceptionPage();
        app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
        {
          HotModuleReplacement = true,
          ReactHotModuleReplacement = true
        });
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
