using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
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
        ApprovalSameDayReminder = 9,
        ApprovalDailyReminder = 10,
        Completed = 11,
        KPILock = 12,
        AddDelegation = 13,
        UpdateDelegation = 14,
        DeleteDelegation = 15,
        ExpiredDelegation = 16,


        CreateLevelSubmit = 17,
        CreateLevelAction = 18,
        ChangeRequestSubmit = 19,
        ChangeRequestAction = 20,
        CreateInitiativeFinalApproval = 21,
        ChangeRequestFinalApproval = 22,
        CreateKPIFinalApproval = 23,
        ChangeRequestSubmitSMO = 24,
        CreateMitigationPlan=25,
        CreateMitigationPlanAction = 26,
        MitigationPlanResponse = 27,
        MitigationPlanReturn = 28,
    }
}
