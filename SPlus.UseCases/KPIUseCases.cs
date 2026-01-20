using Newtonsoft.Json;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.UseCases
{
    public class KPIUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly KPIBLL KPIBLL;
        private readonly KPITypeBLL KPITypeBLL;
        private readonly RequestBLL RequestBLL;
        private readonly NotificationConfigurationBLL NotificationConfigurationBLL;
        private readonly UserBLL UserBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        public KPIUseCases()
        {
            KPIBLL = _Container.GetInstance<KPIBLL>();
            RequestBLL = _Container.GetInstance<RequestBLL>();
            NotificationConfigurationBLL = _Container.GetInstance<NotificationConfigurationBLL>();
            UserBLL = _Container.GetInstance<UserBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
        }

        #region Create
        public CAKPIDTO Create(CAKPIDTO kpi, string userName)
        {
            if (string.IsNullOrWhiteSpace(kpi.ArabicName))
                kpi.ArabicName = kpi.EnglishName;



            KPI KPI = AutoMapper.Mapper.Map<KPI>(kpi);
            if (KPI.DivisionalObjective != null)
            {
                KPI.OrgStructureID = null;
            }
            return AutoMapper.Mapper.Map<CAKPIDTO>(KPIBLL.Create(KPI, userName));
        }
        public KPICommentDTO CreateComment(KPICommentDTO kpiComment, string userName)
        {
            List<UserListDTO> Users = AutoMapper.Mapper.Map<List<UserListDTO>>(UserBLL.Read());
            KPIComment KPIComment = AutoMapper.Mapper.Map<KPIComment>(kpiComment);
            var res = KPIBLL.CreateComment(KPIComment, userName);
            var result = AutoMapper.Mapper.Map<KPICommentDTO>(res);
            result.CreatedByModel = Users.Where(a => a.UserName.ToLower() == res.CreatedBy.ToLower()).FirstOrDefault();
            return result;
        }


        public List<KPIAffectDTO> ConnectWithAffect(List<KPIAffectDTO> _KPIAffectDTO, bool Delete)
        {

            KPIAffectDTO TempAffect = null;
            List<KPIAffectDTO> TempList = new List<KPIAffectDTO>();
            foreach (var item in _KPIAffectDTO)
            {
                TempAffect = new KPIAffectDTO();
                if (item.Affected != 0)
                {
                    TempAffect.KPIID = item.Affected;
                    TempAffect.Effecting = item.KPIID;
                    TempList.Add(TempAffect);
                }
                if (item.Effecting != 0)
                {
                    TempAffect.KPIID = item.Effecting;
                    TempAffect.Affected = item.KPIID;
                    TempList.Add(TempAffect);
                }
            }
            _KPIAffectDTO.AddRange(TempList);


            List<KPIAffect> KPIAffect = AutoMapper.Mapper.Map<List<KPIAffect>>(_KPIAffectDTO);
            var res = KPIBLL.ConnectWithAffect(KPIAffect, Delete);
            var result = AutoMapper.Mapper.Map<List<KPIAffectDTO>>(res);
            return result;
        }

        #endregion

        #region Read


        public List<KPIAffectDTO> ReadWithAffect()
        {
            var kpisAffect = KPIBLL.ReadWithAffect();


            List<KPIAffectDTO> KPIs = AutoMapper.Mapper.Map<List<KPIAffectDTO>>(kpisAffect);
            return KPIs;
        }


        public List<CAKPIListingDTO> Read(string username)
        {
            var kpis = KPIBLL.Read(username, null);
            foreach (var item in kpis)
            {
                item.IsDeletable = true;

                if (KPIBLL.IsInGracePeriod(item))
                    item.AllowLock = false;
                else
                    item.AllowLock = true;

            }
            List<CAKPIListingDTO> KPIs = AutoMapper.Mapper.Map<List<CAKPIListingDTO>>(kpis);
            return KPIs;
        }



        public List<KPIDetailsDTO> ReadReportKPIs(string username)
        {
            var kpis = KPIBLL.ReadReportKPIs(username, null);



            List<KPIDetailsDTO> KPIs = AutoMapper.Mapper.Map<List<KPIDetailsDTO>>(kpis);
            return KPIs;
        }

        public List<CAKPIListingDTO> ReadByType(string username, string Type, int? Year)
        {
            var kpis = KPIBLL.Read(username, Year);
            if (Year.HasValue)
            {
                kpis = kpis.Where(w => w.StartDate.Year == Year.Value).ToList();
            }
            if (Type.ToLower() == "divisional")
            {
                kpis = kpis.Where(a => !a.StrategicObjectiveID.HasValue && a.DivisionalObjectiveID.HasValue).ToList();

            }
            else if (Type.ToLower() == "strategic")
            {

                kpis = kpis.Where(a => a.StrategicObjectiveID.HasValue && !a.DivisionalObjectiveID.HasValue).ToList();

            }

            foreach (var item in kpis)
            {
                //item.IsDeletable = false;
                //item.IsEditable = false;

                if (KPIBLL.IsInGracePeriod(item))
                    item.AllowLock = false;
                else
                    item.AllowLock = true;

            }
            List<CAKPIListingDTO> KPIs = AutoMapper.Mapper.Map<List<CAKPIListingDTO>>(kpis);
            return KPIs;
        }
        public KPIDetailsDTO ReadByID(int id, string username)
        {
            var kpi = KPIBLL.ReadByID(id, username);
            if (kpi != null)
            {
                var kpiTypes = KPITypeBLL.ReadByIDForKPIDetails(kpi.KPITypeID);
                kpi.KPIType = kpiTypes;
                var orgStructures = OrgStructureBLL.Read(username, null);

                if (kpi.OrgStructure != null && kpi.OrgStructure.ParentID.HasValue)
                {
                    kpi.OrgStructure = OrgStructureBLL.BuildUpwardTree(kpi.OrgStructure, orgStructures);
                }

                if (kpi.DivisionalObjective != null && kpi.DivisionalObjective.OrgStructure != null && kpi.DivisionalObjective.OrgStructure.ParentID.HasValue)
                {
                    kpi.DivisionalObjective.OrgStructure = OrgStructureBLL.BuildUpwardTree(kpi.DivisionalObjective.OrgStructure, orgStructures);
                }


                KPIDetailsDTO KPI = AutoMapper.Mapper.Map<KPIDetailsDTO>(kpi);

                if (KPI.KPIMeasures.Any(e => e.OutOfTarget > 100))
                {
                    foreach (var item in KPI.KPIMeasures.Where(e => e.OutOfTarget > 100).ToList())
                    {
                        item.OutOfTarget = 100;
                    }
                }
                else if (KPI.KPIMeasures.Any(e => e.OutOfTarget < 0))
                {
                    foreach (var item in KPI.KPIMeasures.Where(e => e.OutOfTarget < 100).ToList())
                    {
                        item.OutOfTarget = 0;
                    }
                }
                var requests = RequestBLL.GetAllRequests();
                var definition = new { ID = 0 };
                if (username.ToLower() == kpi.ChampionModel.UserName.ToLower())
                {

                    // TO DO AS 
                    var crRequest = requests.Where(w => JsonConvert.DeserializeAnonymousType(w.Form, definition).ID == id &&
                               (w.WorkflowID == Convert.ToInt32(ConfigurationManager.AppSettings["KPIChangeRequestWorkflowID"])) &&
                             (w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.Return || w.Status == (int)EnumWFStatuses.New)).FirstOrDefault();

                    KPI.CanCR = crRequest is null ? true : false;
                }
                else
                    KPI.CanCR = false;

                return KPI;
            }
            return null;
        }


        public List<int> GetValidYears(string username)
        {
            var validYears = KPIBLL.GetValidYears(username);

            return validYears;
        }

        public List<KPIDetailsDTO> ReadChangeRequestKpis(string username, int? Year)
        {
            var kpis = KPIBLL.ReadReportKPIs(username, Year);
            if (kpis != null)
            {
                if (Year.HasValue)
                {
                    kpis = kpis.Where(w => w.StartDate.Year == Year.Value).ToList();
                }

                List<KPIDetailsDTO> KPIs = AutoMapper.Mapper.Map<List<KPIDetailsDTO>>(kpis);
                var requests = RequestBLL.GetAllRequests();

                foreach (var KPI in KPIs)
                {
                    var definition = new { ID = 0 };
                    if (username.ToLower() == KPI.ChampionModel.UserName.ToLower())
                    {

                        // TO DO AS 
                        var crRequest = requests.Where(w => JsonConvert.DeserializeAnonymousType(w.Form, definition).ID == KPI.ID &&
                                   (w.WorkflowID == Convert.ToInt32(ConfigurationManager.AppSettings["KPIChangeRequestWorkflowID"])) &&
                                 (w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.Return || w.Status == (int)EnumWFStatuses.New)).FirstOrDefault();

                        KPI.CanCR = crRequest is null ? true : false;
                    }
                    else
                        KPI.CanCR = false;


                }
                return KPIs.Where(a => a.CanCR).ToList();
            }
            return null;
        }

        public List<CAKPIListingDTO> ReadMitigationActionKpis(int? Year)
        {
            var kpis = KPIBLL.ReadMitigationActionKpis(Year);
            if (kpis != null)
            {
                if (Year.HasValue)
                {
                    kpis = kpis.Where(w => w.StartDate.Year == Year.Value).ToList();
                }

                List<CAKPIListingDTO> KPIs = AutoMapper.Mapper.Map<List<CAKPIListingDTO>>(kpis);

                return KPIs.ToList();
            }
            return null;
        }
        public List<KPICommentDTO> ReadComments(int KPIID, string userName)
        {
            List<UserListDTO> Users = AutoMapper.Mapper.Map<List<UserListDTO>>(UserBLL.Read());
            List<KPIComment> kpiComments = KPIBLL.ReadComments(KPIID, userName);
            List<KPICommentDTO> kpiCommentsDTO = AutoMapper.Mapper.Map<List<KPICommentDTO>>(kpiComments);
            foreach (var kPIComment in kpiCommentsDTO)
            {
                kPIComment.CreatedByModel = Users.Where(w => w.UserName.ToLower() == kPIComment.CreatedBy.ToLower()).FirstOrDefault();
            }
            return kpiCommentsDTO;


        }

        public List<KPICardDTO> KPICardByObjectiveID(int ID, string userName, bool onlyMine, int? Year)
        {
            List<KPI> kpis = KPIBLL.Read(userName, Year).Where(a => a.StrategicObjectiveID.HasValue && !a.DivisionalObjectiveID.HasValue).ToList();

            if (Year.HasValue)
            {
                kpis = kpis.Where(w => w.StartDate.Year == Year.Value).ToList();
            }

            #region OnlyMine
            if (onlyMine)
            {
                kpis = kpis.Where(w => w.Champion.ToLower() == userName.ToLower() || (w.Owner != null && w.Owner.ToLower() == userName.ToLower())).ToList();
            }
            #endregion

            kpis = kpis.Where(w => w.StrategicObjectiveID.Value == ID).ToList();

            List<KPICardDTO> finalKPIs = AutoMapper.Mapper.Map<List<KPICardDTO>>(kpis);
            return finalKPIs;
        }


        public List<KPICardDTO> KPICardByDivisionalObjective(int ID, string userName, bool onlyMine, int? Year)
        {
            List<KPI> kpis = KPIBLL.Read(userName, Year).Where(a => !a.StrategicObjectiveID.HasValue && a.DivisionalObjectiveID.HasValue).ToList();
            if (Year.HasValue)
            {
                kpis = kpis.Where(w => w.StartDate.Year == Year.Value).ToList();
            }
            #region OnlyMine
            if (onlyMine)
            {
                kpis = kpis.Where(w => w.Champion.ToLower() == userName.ToLower() || (w.Owner != null && w.Owner.ToLower() == userName.ToLower())).ToList();
            }
            #endregion

            kpis = kpis.Where(w => w.DivisionalObjectiveID.Value == ID).ToList();

            List<KPICardDTO> finalKPIs = AutoMapper.Mapper.Map<List<KPICardDTO>>(kpis);
            return finalKPIs;
        }
        #endregion

        #region Update

        public CAKPIDTO Update(CAKPIDTO kpi, string username)
        {
            //As commeted no update service> update will be from CR
            //if (string.IsNullOrWhiteSpace(kpi.ArabicName))
            //    kpi.ArabicName = kpi.EnglishName;

            KPI KPI = AutoMapper.Mapper.Map<KPI>(kpi);
            var res = KPIBLL.Update(KPI, username);
            RequestBLL.UpdateRequestApprovers(res);
            return AutoMapper.Mapper.Map<CAKPIDTO>(res);


        }

        public KPICommentDTO UpdateComment(KPICommentDTO kpiComment, string userName)
        {
            List<UserListDTO> Users = AutoMapper.Mapper.Map<List<UserListDTO>>(UserBLL.Read());
            KPIComment _KPIComment = AutoMapper.Mapper.Map<KPIComment>(kpiComment);
            var res = KPIBLL.UpdateComment(_KPIComment);


            var result = AutoMapper.Mapper.Map<KPICommentDTO>(res);
            result.CreatedByModel = Users.Where(a => a.UserName.ToLower() == res.CreatedBy.ToLower()).FirstOrDefault();
            return result;


        }



        #endregion

        #region Delete

        public bool Delete(int id)
        {
            return KPIBLL.Delete(id);
        }
        public bool DeleteComment(int id)
        {
            return KPIBLL.DeleteComment(id);
        }

        #endregion


        #region Processes

        public bool UnlockKPI(int id)
        {
            return KPIBLL.UnlockKPI(id);
        }
        public bool LockKPI(int id)
        {
            return KPIBLL.LockKPI(id);
        }

        #endregion

        #region Hanfire
        public void OpenKPIsForUpdate()
        {
            var kpis = KPIBLL.ReadForHangfire();
            List<KPI> KPIsToUpdate = new List<KPI>();
            List<Request> requests = RequestBLL.GetRequests(LevelTypeEnum.KPI, EnumWFBaseWorkflows.Update).ToList();
            List<Request> PendingRequests = requests.Where(w => w.Status == (int)EnumWFStatuses.Pending).ToList();
            List<Request> rejectedRequests = requests.Where(w => w.Status == (int)EnumWFStatuses.Rejected).ToList();
            foreach (KPI KPI in kpis)
            {
                if(KPI.ID== 4296 || KPI.ID == 4297)
                {

                }
             //  KPI KPI = KPIBLL.ReadForHangfireByID(kpi.ID);
                List<Request> OccupiedRequests = new List<Request>();
                foreach (Request request in PendingRequests)
                {
                    UpdateKPIForm Form = request.UpdateKPIForm;
                    if (KPI.KPIMeasures.Any(a => a.ID == Form.RelatedID))
                    {
                        OccupiedRequests.Add(request);
                    }
                }
                var measures = KPI.KPIMeasures.Where(a => a.DueDate.Date <= DateTime.Now.Date && a.Status == "NA").OrderBy(a => a.ID).ToList();

                bool isFree = !OccupiedRequests.Where(w => measures.Any(a => a.ID == w.UpdateKPIForm.RelatedID)).Any()
                    || OccupiedRequests.Where(w => measures.Any(a => a.ID == w.UpdateKPIForm.RelatedID)).Any();


                if (measures.Count() > 0 && isFree)
                {
                    if (!KPI.RequireUpdate)
                    {
                        KPI.RequireUpdate = true;
                    }
                    foreach (var measure in measures.Where(a => !a.AllowUpdate))
                    {
                        if (measure != null)
                        {
                            var request = requests.Where(w => w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.New).Where(w => w.UpdateKPIForm.RelatedID == measure.ID).FirstOrDefault();
                            //var request = RequestBLL.GetRequestsByFormRelatedID(measure.ID, LevelTypeEnum.KPI).Where(w => w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.New).FirstOrDefault();
                            if (!measure.AllowUpdate && request == null)
                            {
                                KPI.KPIMeasures.Where(a => a.ID == measure.ID).FirstOrDefault().AllowUpdate = true;
                                if (measure.DueDate.Date == DateTime.Now.Date)
                                    Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.UpdateSameDayReminder, LevelTypeEnum.KPI); });
                            }
                        }
                    }
                    foreach (var measure in measures.Where(a => a.AllowUpdate))
                    {
                        if (measure != null)
                        {
                            var request = requests.Where(w => w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.New || w.Status == (int)EnumWFStatuses.Completed).Where(w => w.UpdateKPIForm.RelatedID == measure.ID).FirstOrDefault();
                            //var request = RequestBLL.GetRequestsByFormRelatedID(measure.ID, LevelTypeEnum.KPI).Where(w => w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.New || w.Status == (int)EnumWFStatuses.Completed).FirstOrDefault();
                            if (measure.AllowUpdate && request != null)
                            {
                                KPI.KPIMeasures.Where(a => a.ID == measure.ID).FirstOrDefault().AllowUpdate = false;
                            }
                        }
                    }

                    if (KPIBLL.IsInGracePeriod(KPI))
                    {
                        KPI.ManualUnLock = false;
                        KPI.UnlockDate = null;
                        KPI.IsLocked = false;
                    }
                    else if (!KPI.IsLocked)
                    {
                        if (!KPI.ManualUnLock)
                        {
                            if (!rejectedRequests.Any(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)))
                            {
                              
                                KPI.IsLocked = true;
                                //Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                            }
                            else
                            {
                                var rejectrequest = rejectedRequests.Where(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)).FirstOrDefault();
                                if (rejectrequest != null && KPIBLL.IsInGracePeriod_Rejected(rejectrequest.Modified.Date, KPI))
                                {
                                    KPI.IsLocked = false;
                                }
                                else
                                {
                                    KPI.IsLocked = true;
                                    //Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                                }

                            }
                        }
                        else
                        {
                            if (!rejectedRequests.Any(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)))
                            {

                                KPI.IsLocked = false;
                                //Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                            }
                            else
                            {
                                var rejectrequest = rejectedRequests.Where(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)).FirstOrDefault();
                                if (rejectrequest != null && KPIBLL.IsInGracePeriod_Rejected(rejectrequest.Modified.Date, KPI))
                                {
                                    KPI.IsLocked = false;
                                }

                                else
                                {
                                    KPI.IsLocked = true;
                                    //Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                                }

                            }
                        }

                            //KPI.IsLocked = false;
                    }
                    else
                    {


                        if (!KPI.ManualUnLock)
                        {
                            if (!rejectedRequests.Any(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)))
                            {

                                KPI.IsLocked = true;
                                //Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                            }
                            else
                            {
                                var rejectrequest = rejectedRequests.Where(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)).FirstOrDefault();
                                if (rejectrequest != null && KPIBLL.IsInGracePeriod_Rejected(rejectrequest.Modified.Date, KPI))
                                {
                                    KPI.IsLocked = false;
                                }
                                else
                                {
                                    KPI.IsLocked = true;
                                    //Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                                }

                            }
                        }
                        else
                        {
                            if (!rejectedRequests.Any(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)))
                            {

                                KPI.IsLocked = false;
                               // Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                            }
                            else
                            {
                                var rejectrequest = rejectedRequests.Where(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)).FirstOrDefault();
                                if (rejectrequest != null && KPIBLL.IsInGracePeriod_Rejected(rejectrequest.Modified.Date, KPI))
                                {
                                    KPI.IsLocked = false;
                                }
                                else
                                {
                                    KPI.IsLocked = true;
                                    //Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                                }

                            }
                        }


                        //if (!rejectedRequests.Any(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)))
                        //{

                        //    KPI.IsLocked = true;
                        //    Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
                        //}
                        //else
                        //{
                        //    var rejectrequest = rejectedRequests.Where(a => KPI.KPIMeasures.Any(measure => measure.ID == a.UpdateKPIForm?.RelatedID)).FirstOrDefault();
                        //    if (rejectrequest != null && KPIBLL.IsInGracePeriod_Rejected(rejectrequest.Modified.Date, KPI))
                        //    {
                        //        KPI.IsLocked = false;
                        //    }
                        //    else
                        //        KPI.IsLocked = true;

                        //}
                    }
                }
                else
                {
                    
                    continue;
                }

                KPIsToUpdate.Add(KPI);
            }
            KPIBLL.OpenKPIForUpdate(KPIsToUpdate);

            #region Commented
            //var kpis = KPIBLL.ReadForHangfire();
            //List<KPI> KPIsToUpdate = new List<KPI>();
            //List<Request> requests = RequestBLL.GetRequests(LevelTypeEnum.KPI, EnumWFBaseWorkflows.Update).Where(w => w.Status == (int)EnumWFStatuses.Pending).ToList();
            //foreach (KPI kpi in kpis)
            //{
            //    KPI KPI = KPIBLL.ReadForHangfireByID(kpi.ID);
            //    List<Request> OccupiedRequests = new List<Request>();
            //    foreach (Request request in requests)
            //    {
            //        UpdateKPIForm Form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
            //        if (kpi.KPIMeasures.Any(a => a.ID == Form.RelatedID))
            //        {
            //            OccupiedRequests.Add(request);
            //        }
            //    }
            //    var measures = KPI.KPIMeasures.Where(a => a.DueDate.Date <= DateTime.Now.Date && a.Status == "NA").OrderBy(a => a.ID).ToList();

            //    bool isFree = !OccupiedRequests.Where(w => measures.Any(a => a.ID == JsonConvert.DeserializeObject<UpdateKPIForm>(w.Form).RelatedID)).Any()
            //        || OccupiedRequests.Where(w => measures.Any(a => a.ID == JsonConvert.DeserializeObject<UpdateKPIForm>(w.Form).RelatedID)).Any();

            //    if (measures.Count() > 0 && isFree)
            //    {
            //        if (!KPI.RequireUpdate)
            //        {
            //            KPI.RequireUpdate = true;
            //        }
            //        foreach (var measure in measures.Where(a => !a.AllowUpdate))
            //        {
            //            if (measure != null)
            //            {
            //                var request = RequestBLL.GetRequestsByFormRelatedID(measure.ID, LevelTypeEnum.KPI, EnumWFBaseWorkflows.Update).Where(w => w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.New).FirstOrDefault();
            //                if (!measure.AllowUpdate && request == null)
            //                {
            //                    KPI.KPIMeasures.Where(a => a.ID == measure.ID).FirstOrDefault().AllowUpdate = true;
            //                    if (measure.DueDate.Date == DateTime.Now.Date)
            //                        Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.UpdateSameDayReminder, LevelTypeEnum.KPI); });
            //                }
            //            }
            //        }

            //        foreach (var measure in measures.Where(a => a.AllowUpdate))
            //        {
            //            if (measure != null)
            //            {
            //                var request = RequestBLL.GetRequestsByFormRelatedID(measure.ID, LevelTypeEnum.KPI, EnumWFBaseWorkflows.Update).Where(w => w.Status == (int)EnumWFStatuses.Pending || w.Status == (int)EnumWFStatuses.New || w.Status == (int)EnumWFStatuses.Completed).FirstOrDefault();
            //                if (measure.AllowUpdate && request != null)
            //                {
            //                    KPI.KPIMeasures.Where(a => a.ID == measure.ID).FirstOrDefault().AllowUpdate = false;
            //                }
            //            }
            //        }

            //        if (KPIBLL.IsInGracePeriod(KPI))
            //        {
            //            KPI.ManualUnLock = false;
            //            KPI.UnlockDate = null;
            //            KPI.IsLocked = false;
            //        }
            //        else if (!KPI.IsLocked)
            //        {
            //            if (!KPI.ManualUnLock)
            //            {
            //                KPI.IsLocked = true;
            //                Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(KPI.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI); });
            //            }
            //            else
            //                KPI.IsLocked = false;
            //        }

            //        KPIsToUpdate.Add(KPI);
            //    }

            //}
            //KPIBLL.OpenKPIForUpdate(KPIsToUpdate);
            #endregion
        }

        //public void LockUnlockKPIs()
        //{
        //    var kpis = KPIBLL.ReadForHangfire();
        //    foreach (KPI kpi in kpis)
        //    {
        //        if(kpi.ID == 62)
        //        {

        //        }
        //        if (KPIBLL.IsInGracePeriod(kpi))
        //        {
        //            kpi.IsLocked = false;
        //           // KPIBLL.LockUnlockKPI(kpi);

        //        }
        //        else if (kpi.ManualUnLock)
        //        {
        //           //KPIBLL.LockUnlockKPI(kpi);
        //            kpi.IsLocked = false;
        //        }
        //        else if (!kpi.IsLocked)
        //        {
        //            if (!kpi.ManualUnLock)
        //            {
        //                kpi.IsLocked = true;
        //                KPIBLL.LockUnlockKPI(kpi);
        //                Task.Run(() => { NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI, "Reminder"); });
        //                //NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, 0, 0, enumNotificationEventType.KPILock, LevelTypeEnum.KPI, "Reminder");
        //            }
        //        }
        //    }
        //    KPIBLL.LockUnlockKPIs(kpis);
        //}

        public void KPIUpdateReminder()
        {
            var kpis = KPIBLL.KPIUpdateReminder();
            foreach (KPI kpi in kpis)
            {
                if (kpi.ID == 62)
                {

                }
                //Before reminder
                KPIMeasure beforeMeasure = kpi.KPIMeasures.OrderBy(a => a.ID).Where(a => a.DueDate > DateTime.Now.Date).FirstOrDefault();
                if (beforeMeasure != null)
                {
                    if (DateHelper.GetEndDateWorkingDays(DateTime.Now.Date, kpi.KPIType.ReminderConfiguration.BeforeReminder).Date == beforeMeasure.DueDate.Date)
                    {
                        //Send Before Reminder
                        Task.Run(() =>
                        {
                            NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, beforeMeasure.ID, 0, enumNotificationEventType.BeforeUpdateReminder, LevelTypeEnum.KPI, "Reminder");
                        });

                    }
                }
                KPIMeasure currentMeasure = kpi.KPIMeasures.OrderBy(a => a.ID).Where(a => a.DueDate <= DateTime.Now.Date).LastOrDefault();
                if (currentMeasure != null)
                {
                    if (DateHelper.GetEndDateWorkingDays(currentMeasure.DueDate.Date, kpi.KPIType.ReminderConfiguration.FirstReminder).Date == DateTime.Now.Date)
                    {
                        //Send First Reminder
                        Task.Run(() =>
                        {
                            NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, currentMeasure.ID, 0, enumNotificationEventType.UpdateFirstReminder, LevelTypeEnum.KPI, "Reminder");
                        });
                    }
                    else if (DateHelper.GetEndDateWorkingDays(currentMeasure.DueDate.Date, kpi.KPIType.ReminderConfiguration.SecondReminder).Date == DateTime.Now.Date)
                    {
                        //Send Second Reminder
                        Task.Run(() =>
                        {
                            NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, currentMeasure.ID, 0, enumNotificationEventType.UpdateSecondReminder, LevelTypeEnum.KPI, "Reminder");
                        });
                    }
                    else if (DateHelper.GetEndDateWorkingDays(currentMeasure.DueDate.Date, kpi.KPIType.GracePeriod).Date == DateTime.Now.Date)
                    {
                        //Send Second Reminder
                        Task.Run(() =>
                        {
                            NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, currentMeasure.ID, 0, enumNotificationEventType.UpdateLastReminder, LevelTypeEnum.KPI, "Reminder");
                        });
                    }

                }
            }
        }
        #endregion


        public bool UpdateKPIPeriod_FromMigration()
        {
            return KPIBLL.UpdateKPIPeriod_FromMigration();
        }
    }
}
