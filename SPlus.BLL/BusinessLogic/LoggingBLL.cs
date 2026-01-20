using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

using SPlus.DataAccess;
using SPlus.Model.Domain;
using AutoMapper;
using System.IO;
using System.Web;
using System.Web.Hosting;
using model = SPlus.Model;
using Exception = SPlus.Model.Domain.Exception;
using SPlus.Helper;

namespace SPlus.BLL
{
    public class LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();
        static string Date;
        static string Time;
        readonly string path;
        readonly string dirPath;

        Exception exception;

        private readonly IUnitOfWorkFactory _factory;
        public LoggingBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
            Date = DateTime.Now.ToString("yyyyMMdd");
            Time = DateTime.Now.ToString("HHmm");
            dirPath = HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}"));
            path = HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}/log_{Date}.txt"));
            exception = new Exception();
        }
        public async Task<int> Log(IEnumerable<string> Token, object item, model.AuditTrailActionENums action, int id = 0)
        {
            var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
            using (var dataAccess = _factory.Create())
            {
                AuditTrail audit = new AuditTrail();
                if (id == 0)
                {
                    audit.ItemID = (int)item.GetType().GetProperties()
                                .Where(w => w.Name?.ToLower() == "id")
                                .Select(s => s.GetValue(item))
                                .FirstOrDefault();

                    audit.Action = action.ToString();
                    audit.ItemType = item.GetType().Name;
                    audit.UserName = Credential[0];
                }
                else
                {
                    audit.ItemID = id;
                    audit.Action = action.ToString();
                    audit.ItemType = item.ToString();
                    audit.UserName = Credential[0];
                }

                dataAccess.AuditTrail.Save(audit);
                dataAccess.Complete();
                dataAccess.Dispose();
                return audit.ID;

            }
        }
        public void CreateException(string Title, string ControllerName, string Message, string MethodName)
        {

            exception.Title = Title;
            exception.Controller = ControllerName;
            exception.Message = Message;
            exception.Function = MethodName;
            exception.Created = DateTime.Now;
            exception.Modified = DateTime.Now;
            LogException(exception);
               
        }

      
        private void LogException(Exception exception)
        {
            findandcreate();
            File.AppendAllText(path,
                DateTime.Now + "\t" +
                exception.Controller + Environment.NewLine
                + exception.Function + Environment.NewLine
                + exception.Message + Environment.NewLine
                + Environment.NewLine);
        }
        private void findandcreate()
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }


    }
}
