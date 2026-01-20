using Microsoft.Owin;
using Owin;
using System;
using Hangfire;
using Hangfire.SqlServer;
using System.Collections.Generic;
using System.Web.Http;
using System.Configuration;

using GlobalConfiguration = Hangfire.GlobalConfiguration;

[assembly: OwinStartup(typeof(Tahakum.SPlus.HangFireAPI.Startup))]

namespace Tahakum.SPlus.HangFireAPI
{
    public class Startup : HttpConfiguration
    {
        public static void Configuration(IAppBuilder app)
        {
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            Jobs jobs = new Jobs(); 
        }
        private static IEnumerable<IDisposable> GetHangfireServers()
        {
            Hangfire.GlobalConfiguration.Configuration
                          .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                          .UseSimpleAssemblyNameTypeSerializer()                          
                          .UseRecommendedSerializerSettings()
                          .UseSqlServerStorage(ConfigurationManager.AppSettings["ConnectionString"], new SqlServerStorageOptions
                          {                              
                              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                              QueuePollInterval = TimeSpan.Zero,
                              UseRecommendedIsolationLevel = true,                              
                              DisableGlobalLocks = true                              
                          }); 
            yield return new BackgroundJobServer();
        }
    }
}
