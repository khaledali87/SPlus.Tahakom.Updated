using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public enum NotificationEnums
    {
        Pending = 1,
        Closed = 2,
    }
    public enum NotificationReceivers
    {
        Champion = 1, // The KPI Champion
        Owner=2, // The KPI Owner
        Sponsor = 2, // The KPI Sponsor
        NextApprovers = 3, // All Next Steps Approvers
        AllApprovers = 4, // All Steps Approvers Except the Submitter
        CurrentApprover = 5, // Current Step Approver
        NextApprover = 6, // Next Step Approver
        Requester = 7,
    }
}
