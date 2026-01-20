using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public enum enumNotificationEventType
    {
        Submit = 1,
        Approved = 2,
        Rejected = 3,
        BeforeUpdateReminder = 4,
        UpdateSameDayReminder = 5,
        UpdateFirstReminder = 6,
        UpdateSecondReminder = 7,
        UpdateLastReminder = 8,
        ApprovalDailyReminder = 9,
        LastApprovalReminder = 10,
        Completed = 11,
        AddDelegation = 12,
        UpdateDelegation = 13,
        DeleteDelegation = 14,
        ExpiredDelegation = 15
    }
}
