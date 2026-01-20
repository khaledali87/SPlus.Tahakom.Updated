using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public enum WFApproversEnum
    {
        Champion = 1,
        Owner = 2,
        Sponsor = 3,
        Manager = 4,
        NextApprovers = 5, // All Next Steps Approvers
        AllApprovers = 6, // All Steps Approvers Except the Submitter
        CurrentApprover = 7, // Current Step Approver
        NextApprover = 8, // Next Step Approver
        Requester = 9,
    }
}
