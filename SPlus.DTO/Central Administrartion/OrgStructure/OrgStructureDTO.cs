using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OrgStructureDTO : BaseDTO
    {
        public int? ParentID { get; set; }
        public UserListDTO ManagerModel { get; set; }
        public List<OrgStructureDTO> Childrens { get; set; }
        public OrgStructureDTO Parent { get; set; }
        public List<string> Years { get; set; }
        public string Abbreviation { get; set; }
        public bool IsCorporate { get; set; }
        public GroupListDTO Group { get; set; }

    }
}
