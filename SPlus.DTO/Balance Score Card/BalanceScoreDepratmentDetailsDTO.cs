using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BalanceScoreDepratmentDetailsDTO : BaseDTO
    {
        public int? ParentID { get; set; }
        public decimal Performance { get; set; }
        public string Status { get; set; }
        public int KPIsCount { get; set; }
        public List<KPICardDepartmentListDTO> KPIs { get; set; }

        public List<KPICardDepartmentListDTO> AllKPIs { get; set; }
        public KPISpedoMeterChart KPILastAvrage { get; set; }
        public KPISpedoMeterChart KPILastCumulative { get; set; }
        public List<OperationalDepartmentSectorDTO> Perspectives { get; set; }
        public List<DistributionDataByStatus> KPIDistribution
        {
            get
            {
                var tmp = KPIs?.GroupBy(g => new { g.Status })
                .Select(s => new DistributionDataByStatus
                {
                    Status = s.Key.Status,
                    Count = s.Count()
                }).ToList();
                return tmp;
            }
        }
        public List<OperationalBalanceScoreCardKPIHistoryDTO> SumCumulativeAchievement { get; set; }
        public List<OperationalBalanceScoreCardKPIHistoryDTO> AverageCumulativeAchievement { get; set; }
        public List<OperationalBalanceScoreCardKPIHistoryDTO> AverageAchievement { get; set; }
    }

    public class KPISpedoMeterChart
    {
        public decimal Perfromance { get; set; }
        public string Status { get; set; }
    }
    public class DistributionDataByStatus
    {
        public int Count { get; set; }
        public string Status { get; set; }
    }

    public class KPICardDepartmentListDTO : BaseLevelDTO
    {
        public string Status { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Code { get; set; }
        public decimal OutOfTarget { get; set; }
        public decimal AccumulutiveOutOfTarget { get; set; }
        public decimal Value { get; set; }
        public decimal Target { get; set; }
        public int KPITypeID { get; set; }
        public string Direction { get; set; }
        public CAPerspectiveDTO Perspective { get; set; }
        public decimal Weight { get; set; }
        public decimal BusinessUnitWeight { get; set; }
        public decimal? AccumulutiveValue { get; set; }
        public decimal? AccumulutiveTarget { get; set; }

    }

    public class OperationalDepartmentSectorDTO : BaseDTO
    {

        public AttachmentDTO Attachment { get; set; } = null;

        public decimal Weight { get; set; }
        public decimal Performance { get; set; }
    }
}
