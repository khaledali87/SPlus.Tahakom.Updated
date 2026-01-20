using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using SPlus.Model;
using System.Data.Entity;
using Z.EntityFramework.Plus;
using System.Reflection;
using SPlus.Helper;
using StructureMap;

namespace SPlus.BLL
{
    public class StrategyBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public StrategyBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create

        public Strategy Create(Strategy strategy)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                strategy.Created = DateTime.Now;
                strategy.Modified = DateTime.Now;
                dataAccess.Strategy.Save(strategy);
                result = dataAccess.Complete();


                if (strategy.Attachment != null)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
                    List<Attachment> Attachments = new List<Attachment>();

                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == strategy.Attachment.ID).FirstOrDefault();
                    attach.RelatedItemID = strategy.ID;
                    attach.Type = typeof(Strategy).Name;
                    if (attach != null)
                        dataAccess.Attachment.Save(attach, true);
                }
                result = dataAccess.Complete();

                dataAccess.Dispose();
                var res = ReadByID(strategy.ID);
                return res;
            }
        }

        #endregion

        #region Read
        public List<Strategy> Read(int?Year)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Strategy> Strategies = dataAccess.Strategy.Query().IncludeOptimized(a => a.Themes).IncludeOptimized(a=>a.Perspectives).ToList()
                    .Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();
                foreach (var strategy in Strategies)
                {
                    strategy.IsDeletable = IsDeletable(strategy);
                }
                return Strategies.MapAttachment(dataAccess).ToList();
            }
        }


        public Strategy ReadByID(int ID)
        {

            using (var dataAccess = _factory.Create())
            {
                Strategy strategy = dataAccess.Strategy.Query().IncludeOptimized(a => a.Themes).IncludeOptimized(a => a.Perspectives).Where(w => w.ID == ID).FirstOrDefault();
                if (strategy != null)
                {
                    strategy.IsDeletable = IsDeletable(strategy);
                    strategy = strategy.MapAttachment(dataAccess);
                }
                return strategy;
            }
        }
        public Strategy ReadStrategyMap(int ID)
        {
            using (var dataAccess = _factory.Create())
            {
                Strategy strategy = dataAccess.Strategy.Query()
                    .IncludeOptimized(a => a.Themes)
                    .IncludeOptimizedByPath("Themes.StrategicObjectives")                    
                    .Where(w => w.ID == ID).FirstOrDefault();

                if (strategy != null)
                {
                    strategy = strategy.MapAttachment(dataAccess);
                    strategy.Themes = strategy.Themes.MapAttachment(dataAccess).ToList();
                }
                return strategy;
            }
        }
        public List<Strategy> ReadWithTheme(int?Year)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Strategy> Strategies = dataAccess.Strategy.Query()
                    .IncludeOptimized(a => a.Perspectives)
                    .IncludeOptimized(a => a.Themes)
                    .IncludeOptimizedByPath("Themes.StrategicObjectives").ToList()
                    .Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();
                Strategies = Strategies.MapAttachment(dataAccess).ToList();
                foreach (var strategy in Strategies)
                {
                    strategy.IsDeletable = IsDeletable(strategy);
                   
                }


                return Strategies;
            }
        }

        public List<Strategy> ReadWithPerspective(int ?Year)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Strategy> Strategies = dataAccess.Strategy.Query()
                    .IncludeOptimized(a => a.Themes)
                    .IncludeOptimized(a => a.Perspectives)
                     .IncludeOptimizedByPath("Perspectives.KPIs")
                    .ToList()
                    .Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();
                Strategies = Strategies.MapAttachment(dataAccess).ToList();
                foreach (var strategy in Strategies)
                {
                    strategy.IsDeletable = IsDeletable(strategy);

                }


                return Strategies;
            }
        }
        public List<Strategy> ReadWithObjectives(int?Year)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Strategy> Strategies = dataAccess.Strategy.Query()
                    .IncludeOptimized(a => a.Themes)
                    .IncludeOptimizedByPath("Themes.StrategicObjectives")
                    .IncludeOptimizedByPath("Themes.StrategicObjectives.DivisionalObjectives")
                    .IncludeOptimizedByPath("Themes.StrategicObjectives.KPIs")
                                       .ToList()
                                       .Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();

                foreach(var strategy in Strategies)
                {
                    strategy.Themes = strategy.Themes.MapAttachment(dataAccess).ToList();
                }

                return Strategies;
            }
        }

        public List<OrgStructure> ReadWithDivisionalObjectives(int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                List<OrgStructure> OrgStructures = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.KPIs")

                    .ToList()
                    .Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();
                

                return OrgStructures;
            }
        }



        public Strategy ReadWithObjectives(int ID)
        {
            using (var dataAccess = _factory.Create())
            {
                Strategy strategy = dataAccess.Strategy.Query()
                    .IncludeOptimized(a => a.Themes)
                    .IncludeOptimizedByPath("Themes.StrategicObjectives")
                    .Where(w => w.ID == ID).FirstOrDefault();

                if (strategy != null)
                {
                    strategy.Themes = strategy.Themes.MapAttachment(dataAccess).ToList();
                }
                return strategy;
            }
        }
        public List<Strategy> ReadForWeightStrategy()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Strategy> strategies = dataAccess.Strategy.Query()
                    .IncludeOptimized(a => a.Themes)
                    .IncludeOptimizedByPath("Themes.StrategicObjectives")
                    .IncludeOptimizedByPath("Themes.StrategicObjectives.KPIs")
                    .IncludeOptimizedByPath("Themes.StrategicObjectives.KPIs.KPIType")
                    .ToList();
                return strategies.ToList();
            }
        }

        public List<Strategy> ReadForWeightStrategyBystartegicId(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Strategy> strategies = dataAccess.Strategy.Query()
                    .IncludeOptimized(a => a.Themes)
                    .IncludeOptimizedByPath("Themes.StrategicObjectives")
                    .IncludeOptimizedByPath("Themes.StrategicObjectives.KPIs")
                    .IncludeOptimizedByPath("Themes.StrategicObjectives.KPIs.KPIType")
                    .Where(a=>a.Themes.Any(s=>s.StrategicObjectives.Where(i=>i.ID==id).Count()>0))
                    .ToList();
                return strategies.ToList();
            }
        }
        #endregion

        #region Update
        public Strategy Update(Strategy strategy)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                Strategy current = dataAccess.Strategy.Query().Where(w => w.ID == strategy.ID).FirstOrDefault();
                strategy.Modified = DateTime.Now;
                strategy.Created = current.Created;
                dataAccess.Strategy.Save(strategy);
                result = dataAccess.Complete();

                var SourceAttachments = dataAccess.Attachment.Query();
                var attachmentstodelete = current.MapAttachment(dataAccess).Attachment;

                List<Attachment> Attachments = new List<Attachment>();
                Attachment attach = new Attachment();
                attach = SourceAttachments.ToList().Where(w => w.ID == strategy.Attachment.ID).FirstOrDefault();
                if (attach != null)
                {
                    attach.RelatedItemID = strategy.ID;
                    attach.Type = typeof(Strategy).Name;
                    Attachments.Add(attach);
                    dataAccess.Attachment.Save(attach);
                }
                result = dataAccess.Complete();

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

                dataAccess.Dispose();
                var res = ReadByID(strategy.ID);
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
                Strategy strategy = dataAccess.Strategy.Query().IncludeOptimized(a => a.Themes).IncludeOptimized(a=>a.Perspectives).Where(w => w.ID == id).FirstOrDefault();
                if (strategy != null)
                {
                    if (IsDeletable(strategy))
                    {
                        strategy = strategy.MapAttachment(dataAccess);
                        if (strategy.Attachment != null)
                            Task.Run(() => DeleteAttachment(strategy.Attachment));
                        dataAccess.Strategy.Delete(strategy);
                        result = dataAccess.Complete();
                        dataAccess.Dispose();
                        if (result > 0)
                            return true;
                        return false;
                    }
                    else
                        throw new System.Exception("Strategy has related items and cannot be deleted");
                }
                return false;
            }
        }

        #endregion

        private bool IsDeletable(Strategy strategy)
        {
            bool IsDeletable = false;
            if (strategy.Themes.Count() == 0 && strategy.Perspectives.Count() == 0)
            {
                IsDeletable = true;
            }
            else
            {
                IsDeletable = false;
            }
            return IsDeletable;
        }
    }
}
