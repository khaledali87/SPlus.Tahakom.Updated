using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPlus.UseCases;
using System.IO;
using System.Text;
using Hangfire.Common;
using Hangfire.States;

namespace Tahakum.SPlus.HangFireAPI
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public class Jobs
    {
        KPIUseCases kpiUseCases = new KPIUseCases();

        UserUseCases userUseCases = new UserUseCases();
        RequestUseCases requestUseCases = new RequestUseCases();
        DelegationUseCases delegationUserCases = new DelegationUseCases();
        HangfireJobLogCleaner cleaner = new HangfireJobLogCleaner();
        public Jobs()
        {
            #region Minutely
            RecurringJob.AddOrUpdate(() => OpenKPIForUpdate(), Cron.Minutely);
            //RecurringJob.AddOrUpdate(() => AutoApproveRejectedRequests(), Cron.Minutely);


            RecurringJob.AddOrUpdate(() => UpdateUsersFromActiveDirectory(), "*/05 * * * *");
            RecurringJob.AddOrUpdate(() => ExpiredDelegation(), Cron.Daily);
            #endregion

            #region Daily
            RecurringJob.AddOrUpdate(() => KPIUpdateReminder(),  "0 10 * * *");
            RecurringJob.AddOrUpdate(() => KPIApproveDailyReminder(),  "0 10 * * *");
            RecurringJob.AddOrUpdate(() => CleanResources(), Cron.Daily);
            #endregion


            #region Monthly
            RecurringJob.AddOrUpdate(() => SyncKPIHistory(), Cron.Monthly);
            RecurringJob.AddOrUpdate(() => cleaner.DeleteOldJobs(30), Cron.Monthly(), TimeZoneInfo.Local);
            #endregion

            RecurringJob.AddOrUpdate(() => FullSyncKPIHistory(), "0 0 31 2 0");

        }
        #region KPI
        public void OpenKPIForUpdate()
        {
            kpiUseCases.OpenKPIsForUpdate();
        }

        //public void AutoApproveRejectedRequests()
        //{
        //    kpiUseCases.AutoApproveRejectedRequests();
        //}
        //public void LockUnlockKPIs()
        //{
        //    kpiUseCases.LockUnlockKPIs();
        //}
        public void KPIUpdateReminder()
        {
            kpiUseCases.KPIUpdateReminder();
        }
        public void KPIApproveDailyReminder()
        {
            requestUseCases.KPIApproveDailyReminder();
        }

        public void SyncKPIHistory()
        {
            requestUseCases.SyncKPIHistory();
        }
        public void FullSyncKPIHistory()
        {
            requestUseCases.FullSyncKPIHistory();
        }
        #endregion

        public void UpdateUsersFromActiveDirectory()
        {
            userUseCases.UpdateUsersFromActiveDirectory();
        }
        public void ExpiredDelegation()
        {
            delegationUserCases.ExpiredDelegation();
        }
        public void CleanResources()
        {
            userUseCases.CleanResources();
        }
    }
    public class HangfireJobLogCleaner
    {
        public void DeleteOldJobs(int daysOld)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var monitor = JobStorage.Current.GetMonitoringApi();
                var succeededJobs = monitor.SucceededJobs(0, int.MaxValue);

                foreach (var job in succeededJobs)
                {
                    if (job.Value.SucceededAt < DateTime.UtcNow.AddDays(-daysOld))
                    {
                        connection.Dispose(); // Disposes the job storage
                    }
                }
            }
        }
    }
}