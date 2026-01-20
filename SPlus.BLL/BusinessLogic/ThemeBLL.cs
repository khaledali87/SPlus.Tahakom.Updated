using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using StructureMap;

using SPlus.DataAccess;
using SPlus.Model.Domain;
using AutoMapper;
using System.Data.Entity;
using Z.EntityFramework.Plus;

namespace SPlus.BLL
{
    public class ThemeBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public ThemeBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }


        #region Create

        public Theme Create(Theme Theme)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                Theme.Created = DateTime.Now;
                Theme.Modified = DateTime.Now;
                dataAccess.Theme.Save(Theme);
                result = dataAccess.Complete();

                if (Theme.Attachment != null)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
                    List<Attachment> Attachments = new List<Attachment>();

                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == Theme.Attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = Theme.ID;
                        attach.Type = typeof(Theme).Name;
                        if (attach != null)
                            dataAccess.Attachment.Save(attach, true);
                    }
                }
                result = dataAccess.Complete();

                dataAccess.Dispose();
                return Theme;
            }
        }

        #endregion

        #region Read
        public List<Theme> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                List<Theme> Themes = dataAccess.Theme.Query().OrderBy(a => a.Order).ToList();
                return Themes;
            }
        }

        public List<Theme> ReadWithAttachment()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Theme> Themes = dataAccess.Theme.Query().OrderBy(a => a.Order).ToList(); ;

                return Themes.MapAttachment(dataAccess).ToList();

             
                
            }
        }

        public List<Theme> ReadWithStrategicObjectives()
        {
            using (var dataAccess = _factory.Create())
            {
                List<Theme> Themes = dataAccess.Theme.Query()
                    .IncludeOptimized(a => a.StrategicObjectives.OrderBy(s=>s.Order))
                    .OrderBy(a => a.Order).ToList();
                foreach (var Theme in Themes)
                {
                    Theme.IsDeletable = IsDeletable(Theme);
                }
                
                return Themes;
            }
        }

        public Theme ReadByID(int ID)
        {

            using (var dataAccess = _factory.Create())
            {
                Theme Theme = dataAccess.Theme.Query().Where(a => a.ID == ID).IncludeOptimized(a => a.Strategy)
                    .IncludeOptimized(a => a.StrategicObjectives)
                    .FirstOrDefault();

                if(Theme != null)
                {
                    Theme.IsDeletable = IsDeletable(Theme);
                    Theme = Theme.MapAttachment(dataAccess);
                }
                return Theme;
            }
        }

        public List<Theme> ReadForWeightTheme()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Theme> themes = dataAccess.Theme.Query()                      
                    .IncludeOptimized( a => a.StrategicObjectives)
                    .IncludeOptimizedByPath("StrategicObjectives.KPIs")
                    .IncludeOptimizedByPath("StrategicObjectives.KPIs.KPIType")
                    .ToList();
                return themes.ToList();
            }
        }
        #endregion

        #region Update
        public Theme Update(Theme Theme)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {

                Theme current = dataAccess.Theme.Query().Where(w => w.ID == Theme.ID).FirstOrDefault();

                Theme.Created = current.Created;
                Theme.Modified = DateTime.Now;
                Theme.Weight = current.Weight;

                    var SourceAttachments = dataAccess.Attachment.Query();
                    var attachmentstodelete = current.MapAttachment(dataAccess).Attachment;

                    List<Attachment> Attachments = new List<Attachment>();
                    Attachment attach = new Attachment();
                    attach = SourceAttachments.ToList().Where(w => w.ID == Theme.Attachment?.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = Theme.ID;
                        attach.Type = typeof(Theme).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                    }
                    if (attachmentstodelete != null)
                    {
                        if (!Attachments.Select(s => s.ID).Contains(attachmentstodelete.ID))
                        {
                          //  if (attach != null)
                            {
                                Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                            }
                        }
                    }
                

                dataAccess.Theme.Save(Theme);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                var res = ReadByID(Theme.ID);
                return res;
            }

        }

        public List<Theme> Update(List<Theme> Themes)
        {
            int result;

            using (var dataAccess = _factory.Create())
            {
                List<Theme> contexts = dataAccess.Theme.Query().ToList();
                foreach (Theme Theme in Themes)
                {
                    Theme context = contexts.Where(w => w.ID == Theme.ID).FirstOrDefault();
                    context.Weight = Theme.Weight;
                    context.Modified = DateTime.Now;
                    dataAccess.Theme.Save(context);
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return Themes;
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
                Attachments = Attachments.Where(s => Attachment.ID == s.ID).ToList();
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
                Theme Theme = dataAccess.Theme.Query().IncludeOptimized(a=>a.StrategicObjectives).Where(a => a.ID == id).FirstOrDefault();
                if (Theme != null)
                {
                    if (IsDeletable(Theme))
                    {
                        Theme = Theme.MapAttachment(dataAccess);
                        if (Theme.Attachment != null)
                            Task.Run(() => DeleteAttachment(Theme.Attachment));
                        dataAccess.Theme.Delete(Theme);
                        result = dataAccess.Complete();
                        dataAccess.Dispose();
                    }
                    else
                        throw new System.Exception("Theme has related items and cannot be deleted");
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }
        private bool IsDeletable(Theme Theme)
        {
            bool IsDeletable = false;
            if (Theme.StrategicObjectives.Count() > 0)
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
