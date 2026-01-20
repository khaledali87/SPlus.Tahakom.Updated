using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;

namespace Tahakum.SPlus.HangFireAPI
{
    public class HangfireContext : DbContext
    {
        public HangfireContext() : base(ConfigurationManager.AppSettings["ConnectionString"].ToString())
        {
            Database.SetInitializer<HangfireContext>(null);
            
            Database.CreateIfNotExists();
        }
    }
    public static class HangFireGlobalConfiguration
    {
        public static void Triggers()
        {            
            var db = new HangfireContext();            
            BackgroundJobClient backgroundJobClient = new BackgroundJobClient();            
        }
    }
 
}
