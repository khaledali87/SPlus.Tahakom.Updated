using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using SPlus.Helper;
using System.Data.Entity;
using System.Reflection;
using model = SPlus.Model;

namespace SPlus.BLL
{
    public class SystemPerformanceThresholdBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();

        private readonly IUnitOfWorkFactory _factory;
        public SystemPerformanceThresholdBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }


        #region Read
        public List<SystemPerformanceThreshold> SystemPerformanceThreshold()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<SystemPerformanceThreshold> systemPerformanceThresholds = dataAccess.SystemPerformanceThreshold.Query()
                    .Include(a => a.Status).Where(w => w.Code != "NA");
                return systemPerformanceThresholds.ToList();
            }
        }
        public List<SystemPerformanceThreshold> SystemPerformanceThresholdNA()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<SystemPerformanceThreshold> systemPerformanceThresholds = dataAccess.SystemPerformanceThreshold.Query()
                    .Include(a => a.Status);

                Status na = dataAccess.Status.Query().ToList().Where(a => a.Code == "NA").FirstOrDefault();
                SystemPerformanceThreshold th = new SystemPerformanceThreshold();
                th.Status = na;
                systemPerformanceThresholds.ToList().Add(th);
                return systemPerformanceThresholds.ToList();
            }
        }

        #endregion

        #region Update

        public List<SystemPerformanceThreshold> UpdateSystemPerformanceThreshold(List<SystemPerformanceThreshold> systemPerformanceThresholds)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                foreach (var systemPerformanceThreshold in systemPerformanceThresholds)
                {
                    dataAccess.SystemPerformanceThreshold.Save(systemPerformanceThreshold);
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return systemPerformanceThresholds;
            }
        }

        public bool CalculateOperation(decimal Value, decimal Comparer, bool isMax, string Operator)
        {
            if (isMax)
            {
                switch (Operator)
                {
                    case "<": return Value < Comparer;
                    case "<=": return Value <= Comparer;
                    case "=": return Value == Comparer;
                    default: throw new System.Exception("invalid logic");
                }
            }
            else
            {
                switch (Operator)
                {
                    case "<": return Comparer < Value;
                    case "<=": return Comparer <= Value;
                    case "=": return Comparer == Value;
                    default: throw new System.Exception("invalid logic");
                }
            }
        }

        #endregion

        #region Calculation Operation

    
        public bool CalculateSystemOperation(decimal Value, decimal Comparer, bool IsMax, string Operator)
        {
            if (Value != 0)
            {

            }
            if (IsMax)
            {
                switch (Operator)
                {
                    case "<": return Value < Comparer;
                    case "<=": return Value <= Comparer;
                    case "=": return Value == Comparer;
                    default: throw new System.Exception("invalid logic");
                }
            }
            else
            {
                switch (Operator)
                {
                    case "<": return Comparer < Value;
                    case "<=": return Comparer <= Value;
                    case "=": return Comparer == Value;
                    default: throw new System.Exception("invalid logic");
                }
            }

        }
        #endregion
    }
}
