using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using StructureMap;

using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using System.Reflection;
using SPlus.Helper;

namespace SPlus.BLL
{
    public class ParameterBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public ParameterBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create



        #endregion

        #region Read

        #endregion

        #region Update

        public bool UpdateParameterValues(int relatedID, List<Parameter> parameters)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<ParameterValue> parameterValuecontext = dataAccess.ParameterValue.Query().Include(a => a.Measure).ToList();
                IEnumerable<Parameter> parametercontext = dataAccess.Parameter.Query().ToList();

                KPI kpi = dataAccess.KPI.Query()
                    .Include(a => a.KPIMeasures)
                    .Include(a => a.Parameters).ToList()
                    .Where(w => parameters.Select(s => s.KPIID).Contains(w.ID))
                    .FirstOrDefault();

                parameterValuecontext = parameterValuecontext.Where(w => relatedID == w.MeasureID).ToList();

                foreach (var parameter in parameters)
                {
                    foreach (var parameterValue in parameterValuecontext.Where(w => w.ParameterID == parameter.ID && w.MeasureID == relatedID))
                    {
                        parameterValue.Modified = DateTime.Now;
                        parameterValue.Value = parameter.Value;
                        dataAccess.ParameterValue.Save(parameterValue);
                    }
                    var currentParam = parametercontext.Where(w => w.ID == parameter.ID).FirstOrDefault();
                    currentParam.Value = parameter.Value;
                    currentParam.Modified = DateTime.Now;
                    dataAccess.Parameter.Save(currentParam);
                }

                result = dataAccess.Complete();
                dataAccess.Dispose();
            }

            if (result >= 1)
                return true;
            return false;
        }

        #endregion

        #region Delete

        #endregion
    }
}
