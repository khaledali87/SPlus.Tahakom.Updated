using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO.Delegation
{
    public class SystemDelegationDTO
    {
        public List<DelegationDTO> Delegations { get; set; }
        public List<DelegationDTO> AdminActiveDelegations { get; set; }
    }
}
