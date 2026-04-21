using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace SPlus.UseCases
{
    public class AttachmentUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly AttachmentBLL AttachmentBLL;
        private readonly KPIBLL KPIBLL;

        private readonly RequestBLL RequestBLL;
        public AttachmentUseCases()
        {
            AttachmentBLL = _Container.GetInstance<AttachmentBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            RequestBLL = _Container.GetInstance<RequestBLL>();

        }
        #region Create

        public int Create(Attachment attachement)
        {
            return AttachmentBLL.Create(attachement);
        }
        public ResultWrapper<List<int>> UploadAttachment(HttpPostedData data)
        {
            ResultWrapper<List<int>> result = new ResultWrapper<List<int>>();
            Attachment _Attachement;
            List<Attachment> _Attachements = new List<Attachment>();
            foreach (KeyValuePair<string, HttpPostedFile> item in data.Files)
            {
                if (item.Key.Contains("file"))
                {
                    bool isAllowed = MimeTypes.Verify(item.Value.File);
                    //bool isMalicious = false;
                    //bool isSucess = false;
                    //bool isValidImageType = MimeTypes.IsValidImage(item.Value.File);
                    //if (!isValidImageType)
                    //{
                    //    var cancellationToken = new CancellationTokenSource().Token;
                    //    if (item.Value.Filename.Split('.')[1].ToLower() == "pdf")
                    //        (isSucess, isMalicious) = MimeTypes.ValidateAndSanitizePdf(item.Value.File).Result;
                    //}
                    MimeTypes.CheckRealMimeType(item.Value.File);


                    if (isAllowed/* && (!isMalicious || isValidImageType)*/)
                    {
                        _Attachement = new Attachment();
                        _Attachement.Content = new AttachmentContent();
                        _Attachement.Content.Content = item.Value.File;
                        _Attachement.FileName = item.Value.Filename;
                        _Attachement.Size = item.Value.File.Length / 1024;
                        _Attachement.Name = item.Value.Filename.Split('.')[0];
                        _Attachements.Add(_Attachement);
                    }
                    else
                    {
                        result.StatusMessage = "False";
                        return result;
                    }
                }
            }
            if (_Attachements.Count > 0)
            {
                result.Data = Create(_Attachements);
            }
            else
            {
                result.StatusMessage = "No Attachment Uploaded";
                return result;
            }
            return result;
        }
        public List<int> Create(List<Attachment> attachements)
        {
            return AttachmentBLL.Create(attachements);
        }
        #endregion

        #region Read
        public List<AttachmentDTO> Read()
        {
            return AutoMapper.Mapper.Map<List<AttachmentDTO>>(AttachmentBLL.Read());

        }
        public List<KPIAttachmentDTO> ReadByKPIID(int KPIID, string username)
        {
            List<KPIAttachmentDTO> kPIAttachments = new List<KPIAttachmentDTO>();
            var kpi = KPIBLL.ReadByID(KPIID, username);
            var requests = RequestBLL.GetRequests(username);

            if (kpi != null)
            {
                KPIDetailsDTO KPI = Mapper.Map<KPIDetailsDTO>(kpi);
                var requestAttachmnts = requests.SelectMany(s => s.Attachments).ToList();

                var kpiattachments = Mapper.Map<List<KPIAttachmentDTO>>(kpi.Attachments);
                List<Request> finalRequests = new List<Request>();
                foreach (var measure in kpi.KPIMeasures)
                {
                    var definition = new { RelatedID = 0 };
                    var measureRequests = requests.Where(a => JsonConvert.DeserializeAnonymousType(a.Form, definition).RelatedID == measure.ID).ToList();
                    foreach (var item in measureRequests)
                    {
                        if (item != null)
                            finalRequests.Add(item);
                    }
                 
                }

                KPIAttachmentDTO attach = null;
                foreach (var request in finalRequests)
                {
                    //var attachment = requestAttachmnts.Where(w => w != null && w.RelatedItemID == request.ID).FirstOrDefault();
                    var attachments = requestAttachmnts.Where(w => w != null && w.RelatedItemID == request.ID).ToList();
                    foreach (var attachment in attachments)
                    {
                        if (request != null && attachment != null)
                        {
                            var data = (JObject)JsonConvert.DeserializeObject(request.Form);
                            int Type = data.SelectToken("Type").Value<int>();
                            if (Type == (int)LevelTypeEnum.KPI)
                            {
                                UpdateKPIForm Form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                                attach = new KPIAttachmentDTO();
                                attach = Mapper.Map<KPIAttachmentDTO>(attachment);
                                attach.Status = request.Status;
                                attach.Value = Form.Value;
                                kPIAttachments.Add(attach);
                            }
                        }
                    }
                  
                }
                foreach (var item in kpiattachments)
                {
                    item.Status = (int)EnumWFStatuses.NA;
                }
                kPIAttachments.AddRange(kpiattachments);
            }
            return AutoMapper.Mapper.Map<List<KPIAttachmentDTO>>(kPIAttachments);

        }

        public AttachmentDTO ReadByID(int id)
        {
            return AutoMapper.Mapper.Map<AttachmentDTO>(AttachmentBLL.ReadByID(id));
        }

        public HttpResponseMessage GetFile(int ID)
        {
            return AttachmentBLL.GetFile(ID);
        }

        #endregion

        #region Delete

        public bool Delete(int id)
        {
            return AttachmentBLL.Delete(id);
        }

        #endregion
    }
}
