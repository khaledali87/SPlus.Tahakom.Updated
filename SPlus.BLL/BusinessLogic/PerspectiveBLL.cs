using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using StructureMap;
using Z.EntityFramework.Plus;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using System.Reflection;
using SPlus.Helper;

namespace SPlus.BLL
{
    public class PerspectiveBLL : LoggingBLL
    {
                Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public PerspectiveBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create

        public Perspective Create(Perspective perspective)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                perspective.Created = DateTime.Now;
                perspective.Modified = DateTime.Now;
                dataAccess.Perspective.Save(perspective);
                result = dataAccess.Complete();

                if (perspective.Attachment != null)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
                    List<Attachment> Attachments = new List<Attachment>();

                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == perspective.Attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = perspective.ID;
                        attach.Type = typeof(Perspective).Name;
                        if (attach != null)
                            dataAccess.Attachment.Save(attach, true);
                    }
                }
                result = dataAccess.Complete();

                dataAccess.Dispose();
                return perspective;
            }            
        }

        #endregion

        #region Read
        public List<Perspective> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                List<Perspective> Perspectives = dataAccess.Perspective.Query().OrderBy(a => a.Order).ToList();              
                return Perspectives;
            }
        }

        public List<Perspective> ReadWithAttachment()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Perspective> Perspectives = dataAccess.Perspective.Query().OrderBy(a => a.Order).ToList();
                return Perspectives.MapAttachment(dataAccess).ToList();
               
                
            }
        }

        public List<Perspective> ReadWithStrategicObjectives()
        {
            using (var dataAccess = _factory.Create())
            {
                List<Perspective> Perspectives = dataAccess.Perspective.Query()
                    .IncludeOptimized(a => a.KPIs).OrderBy(a => a.Order).ToList();
                foreach (var perspective in Perspectives)
                {
                    perspective.IsDeletable = IsDeletable(perspective);
                }
                return Perspectives;
            }
        }

        public Perspective ReadByID(int ID)
        {

            using (var dataAccess = _factory.Create())
            {
                Perspective perspective = dataAccess.Perspective.Query().Where(a=>a.ID == ID).IncludeOptimized(a=>a.Strategy).IncludeOptimized(a => a.KPIs).FirstOrDefault();
                perspective.IsDeletable = IsDeletable(perspective);
                return perspective.MapAttachment(dataAccess);
            }
        }       
        #endregion

        #region Update
        public Perspective Update(Perspective perspective)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {

                Perspective current = dataAccess.Perspective.Query().Where(w => w.ID == perspective.ID).FirstOrDefault();

                perspective.Created = current.Created;
                perspective.Modified = DateTime.Now;

                var SourceAttachments = dataAccess.Attachment.Query();
                var attachmentstodelete = current.MapAttachment(dataAccess).Attachment;

                List<Attachment> Attachments = new List<Attachment>();
                Attachment attach = new Attachment();
                attach = SourceAttachments.ToList().Where(w => w.ID == perspective.Attachment.ID).FirstOrDefault();
                if (attach != null)
                {
                    attach.RelatedItemID = perspective.ID;
                    attach.Type = typeof(Perspective).Name;
                    Attachments.Add(attach);
                    dataAccess.Attachment.Save(attach);
                }
                if (attachmentstodelete != null)
                {
                    if (!Attachments.Select(s => s.ID).Contains(attachmentstodelete.ID))
                    {
                        if (attach != null)
                        {
                            Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                        }
                    }
                }

                dataAccess.Perspective.Save(perspective);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                var res = ReadByID(perspective.ID);
                return res;
            }

        }

      

        #endregion

        #region Delete

        private async Task<bool> DeleteAttachment(Attachment Attachment)
        {
            int result = 0;

            using (var dataAccess = _factory.Create())
            {
                List<Attachment> Attachments = await dataAccess.Attachment.Query().IncludeOptimized(a => a.Content).ToListAsync();
                Attachments = Attachments.Where(s => Attachment.ID==s.ID).ToList();
                foreach (var attach in Attachments)
                {
                    dataAccess.Attachment.Delete(attach);
                    result = dataAccess.Complete();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }
        public bool Delete(int id) 
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                Perspective perspective = dataAccess.Perspective.Query().IncludeOptimized(a=>a.KPIs).Where(a=>a.ID == id).FirstOrDefault();
                if (perspective != null)
                {
                    if (IsDeletable(perspective))
                    {
                        dataAccess.Perspective.Delete(perspective);
                        result = dataAccess.Complete();
                        dataAccess.Dispose();
                    }
                    else
                        throw new System.Exception("Perspective has related items and cannot be deleted");
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }
        private bool IsDeletable(Perspective perspective)
        {
            bool IsDeletable = false;
            if (perspective.KPIs.Count() > 0)
            {
                IsDeletable = false;
            }
            else
            {
                IsDeletable = true;
            }
            return IsDeletable;
        }

        #endregion
    }
}
