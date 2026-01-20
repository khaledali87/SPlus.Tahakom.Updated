using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAKPIListingDTO : BaseLevelDTO
    {      
        public bool AllowLock { get; set; }
        public bool IsEditable { get; set; }
        public bool IsLocked { get; set; }
        public int KPITypeID { get; set; }
        public string Code { get; set; }
        public int OrgStructureID { get; set; }
        public string Status { get; set; }
        public decimal OutOfTarget { get; set; }
        public decimal AccumulutiveOutOfTarget { get; set; }
        public UserListDTO ChampionModel { get; set; }
        public UserListDTO OwnerModel { get; set; }

    }
}
