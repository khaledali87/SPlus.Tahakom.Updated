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

namespace SPlus.BLL
{
    public class HandshakeBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public HandshakeBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        

        #region Read
        public List<Lookup> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Lookup> lookups = dataAccess.Handshake.Query()
                    .Include(a => a.LookupValues);


                foreach (var lookup in lookups)
                {
                    lookup.LookupValues = lookup.LookupValues.OrderBy(o => o.Order0).ToList();
                }
                return lookups.ToList();
            }
        }

        public Lookup ReadByKey(string key)
        {
            using (var dataAccess = _factory.Create())
            {
                Lookup lookup = dataAccess.Handshake.Query()
                    .Include(a => a.LookupValues)
                    .Where(a=>a.Title.ToLower()==key.ToLower())
                    .FirstOrDefault();

                    lookup.LookupValues = lookup.LookupValues.OrderBy(o => o.Order0).ToList();
                
                return lookup;
            }
        }


        #endregion


    }

}
