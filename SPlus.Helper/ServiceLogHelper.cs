using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Hosting;

namespace SPlus.Helper
{
    public class ServiceLogHelper
    {
        static string Date;
        readonly string pathSer;
        readonly string FolderPath;
        public ServiceLogHelper()
        {
            Date = DateTime.Now.ToString("yyyyMMdd");
            pathSer = HostingEnvironment.MapPath($"~/SplusLogs/PPlusServicesLog/PPLusLog/log_{Date}.txt");
            FolderPath = HostingEnvironment.MapPath("~/SplusLogs/PPlusServicesLog/PPLusLog");
        }
      
        public void findandcreate()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);

                if (!File.Exists(pathSer))
                {
                    File.Create(pathSer).Close();
                }
            }
        }
     
        public void newLog()
        {
            findandcreate();
            File.WriteAllText(pathSer, "");
        }
        public void ServiceLog(string text, string code, string stacktrace = "")
        {
            findandcreate();
            File.AppendAllText(pathSer, DateTime.Now + "\t" + text + Environment.NewLine + "Status Code " + code + Environment.NewLine + stacktrace + Environment.NewLine);
        }
    }

}
