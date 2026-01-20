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
    public class DelegationBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public DelegationBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create

        public Delegation Create(Delegation delegation)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                delegation.Created = DateTime.Now;
                delegation.Modified = DateTime.Now;
                delegation.ModifiedBy = delegation.FromUser;
                delegation.CreatedBy = delegation.FromUser;
                dataAccess.Delegation.Save(delegation);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return delegation;
            }            
        }

        #endregion

        #region Read
        public List<Delegation> Read(string userName)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Delegation> delegations = dataAccess.Delegation.Query()
                    .Include(a => a.CreatedByModel)
                    .Include(a => a.ModifiedByModel)
                    .Include(a => a.DelegatedUser)
                    .Include(a => a.DelegatorUser)
                    .Where(a=>a.DelegatorUser.UserName.ToLower() == userName.ToLower());
                return delegations.ToList();
            }
        }

        public List<Delegation> ReadForAdmin()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Delegation> delegations = dataAccess.Delegation.Query()
                    .IncludeOptimized(a => a.CreatedByModel)
                    .IncludeOptimized(a => a.ModifiedByModel)
                    .IncludeOptimized(a => a.DelegatedUser)
                    .IncludeOptimized(a => a.DelegatorUser).Where(a => a.ToDate >= DateTime.Today.Date);
                return delegations.ToList();
            }
        }
        public Delegation ReadByID(int ID)
        {

            using (var dataAccess = _factory.Create())
            {
                Delegation delegation = dataAccess.Delegation.Query()
                    .Include(a => a.CreatedByModel)
                    .Include(a => a.ModifiedByModel)
                    .Include(a => a.DelegatedUser)
                    .Include(a => a.DelegatorUser)
                    .Where(s => s.ID == ID).SingleOrDefault();
                return delegation;
            }
        }

        public List<Delegation> GetActiveDelegations(string userName)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Delegation> delegations = dataAccess.Delegation.Query()
                    .Include(a => a.CreatedByModel)
                    .Include(a => a.ModifiedByModel)
                    .Include(a => a.DelegatedUser)
                    .Include(a => a.DelegatorUser)
                    .Where(
                    a => a.DelegatedUser.UserName.ToLower() == userName.ToLower()
                    && a.FromDate <= DateTime.Today.Date && a.ToDate >= DateTime.Today.Date
                    );
                var res = delegations.ToList();

                return res;
            }
        }

        #endregion

        #region Update
        public Delegation Update(Delegation delegation)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                Delegation context = dataAccess.Delegation.Get(delegation.ID);

                context.Modified = DateTime.Now;
                context.ModifiedBy = delegation.FromUser;
                context.ToUser = delegation.ToUser;
                context.FromUser = delegation.FromUser;
                context.ToDate = delegation.ToDate;
                context.FromDate = delegation.FromDate;

                dataAccess.Delegation.Save(context);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return delegation;
            }
        }
        public List<Delegation> Update(List<Delegation> delegations)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                List<Delegation> contexts = dataAccess.Delegation.GetAll().ToList();
                foreach (Delegation delegation in delegations)
                {
                    Delegation context = contexts.Where(w => w.ID == delegation.ID).FirstOrDefault();
                    context.Modified = DateTime.Now;
                    context.ModifiedBy = delegation.ModifiedBy;
                    context.ToUser = delegation.ToUser;
                    context.FromUser = delegation.FromUser;
                    context.ToDate = delegation.ToDate;
                    context.FromDate = delegation.FromDate;
                    dataAccess.Delegation.Save(context);
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return delegations;
            }
        }


        #endregion

        #region Delete

        public bool Delete(int id) 
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                Delegation delegation = dataAccess.Delegation.Get(id);
                if (delegation != null)
                {
                    dataAccess.Delegation.Delete(delegation);
                    result = dataAccess.Complete();
                    dataAccess.Dispose();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region Hangfire
        public List<Delegation> GetAllDelegations()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Delegation> delegations = dataAccess.Delegation.Query()
                    .IncludeOptimized(a => a.CreatedByModel)
                    .IncludeOptimized(a => a.ModifiedByModel)
                    .IncludeOptimized(a => a.DelegatedUser)
                    .IncludeOptimized(a => a.DelegatorUser);
                return delegations.ToList();
            }
        }
        #endregion
    }
}
