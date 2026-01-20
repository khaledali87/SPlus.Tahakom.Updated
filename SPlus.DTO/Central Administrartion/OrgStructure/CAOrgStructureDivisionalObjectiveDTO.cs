using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAOrgStructureDivisionalObjectiveDTO : BaseDTO
    {
        public int? ParentID { get; set; }
        public UserListDTO ManagerModel { get; set; }

        public List<CAODivisionalObjectiveKPIDTO> DivisionalObjective { get; set; }
    }
}
