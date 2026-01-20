using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using StructureMap;
using SPlus.Model.Domain;
using SPlus.DataAccess;
using AutoMapper;
using System.Data.Entity;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using Z.EntityFramework.Plus;

namespace SPlus.BLL
{
    public class AttachmentBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public AttachmentBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create

        public int Create(Attachment Attachment)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                Attachment.Created = DateTime.Now;
                Attachment.Modified = DateTime.Now;
                dataAccess.Attachment.Save(Attachment);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return Attachment.ID;
            }
        }
        public List<int> Create(List<Attachment> Attachments)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                foreach (var Attachment in Attachments)
                {
                    Attachment.Created = DateTime.Now;
                    Attachment.Modified = DateTime.Now;

                }
                dataAccess.Attachment.CreateList(Attachments);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return Attachments.Select(s => s.ID).ToList();
            }
        }
        #endregion

        #region Read
        public List<Attachment> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Attachment> Attachments = dataAccess.Attachment.Query().Include(a => a.Content);
                return Attachments.ToList();
            }
        }

        public Attachment ReadByID(int id)
        {

            using (var dataAccess = _factory.Create())
            {
                //Attachment Attachment = dataAccess.Attachment.Query().Include(a => a.Content).Where(w => w.ID == id)
                //    .SingleOrDefault();
                var attachment = dataAccess.Attachment.Query().Include(a => a.Content)
    .SingleOrDefault(a => a.ID == id);
                return attachment;
            }
        }

        public HttpResponseMessage GetFile(int ID)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            Attachment Attachment = ReadByID(ID);
            if (Attachment != null)
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    byte[] fileArray = Attachment.Content.Content.ToArray();
                    if (Attachment.FileName.ToLower().Contains("jpg"))
                    {
                        result.Content = new ByteArrayContent(fileArray.ToArray());
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    }
                    else if (Attachment.FileName.ToLower().Contains("png"))
                    {
                        result.Content = new ByteArrayContent(fileArray.ToArray());
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    }
                    else if (Attachment.FileName.ToLower().Contains("svg"))
                    {
                        result.Content = new ByteArrayContent(fileArray.ToArray());
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/svg+xml");
                    }
                    else
                    {
                        result.Content = new ByteArrayContent(fileArray.ToArray());
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    }
                }
            }
            return result;
        }

        #endregion

        #region Update
        public bool CreateRequestAttachment(List<Attachment> attachments, int requestID, bool isNew)
        {
            bool result = false;
            using (var dataAccess = _factory.Create())
            {
                var SourceAttachments = dataAccess.Attachment.Query();
                List<Attachment> Attachments = new List<Attachment>();

                foreach (var Attachment in attachments)
                {
                    Attachment attach = new Attachment();
                    attach = SourceAttachments.ToList().Where(w => w.ID == Attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = requestID;
                        attach.Type = typeof(Request).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                    }
                }
                result = dataAccess.Complete() > 0;
                var attachmentstodelete = SourceAttachments.Where(w => w.RelatedItemID == requestID && w.Type == typeof(Request).Name).ToList();
                if (attachmentstodelete != null && attachmentstodelete.Count() > 0)
                {
                    attachmentstodelete = attachmentstodelete.Where(w => !Attachments.Select(s => s.ID).Contains(w.ID)).ToList();
                    foreach (var attach in attachmentstodelete)
                    {
                        if (attach != null)
                        {
                            Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                        }
                    }
                }
            }
            return result;
        }

        public bool CreateRequestAttachment(Attachment attachment, int requestID, bool isNew)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                if (attachment != null)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.GetAll().ToList();
                    var attachmentstodelete = SourceAttachments.Where(w => w.RelatedItemID == requestID && w.Type == typeof(Request).Name).ToList();

                    List<Attachment> Attachments = new List<Attachment>();

                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = requestID;
                        attach.Type = typeof(Request).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                    }
                    if (!isNew)
                    {
                        if (attachmentstodelete != null && attachmentstodelete.Count() > 0)
                        {
                            attachmentstodelete = attachmentstodelete.Where(w => !Attachments.Select(s => s.ID).Contains(w.ID)).ToList();
                            foreach (var att in attachmentstodelete)
                            {
                                if (att != null)
                                {
                                    Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                                }
                            }
                        }
                    }
                    result = dataAccess.Complete();
                }

            }
            return result > 0;
        }

        public bool CreateRequestAttachment(Attachment attachment, int requestID)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                if (attachment != null)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.GetAll().ToList();
                    var attachmentstodelete = SourceAttachments.Where(w => w.RelatedItemID == requestID).ToList();

                    List<Attachment> Attachments = new List<Attachment>();

                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = requestID;
                        attach.Type = typeof(Request).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                    }
                    if (attachmentstodelete != null && attachmentstodelete.Count() > 0)
                    {
                        attachmentstodelete = attachmentstodelete.Where(w => !Attachments.Select(s => s.ID).Contains(w.ID)).ToList();
                        foreach (var att in attachmentstodelete)
                        {
                            if (att != null)
                            {
                                Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                            }
                        }
                    }
                    result = dataAccess.Complete();
                }

            }
            return result > 0;
        }

        public bool CreateRequestAttachment_List(List<Attachment> attachments, int requestID)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                if (attachments != null && attachments.Count>0)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.GetAll().ToList();
                    var attachmentstodelete = SourceAttachments.Where(w => w.RelatedItemID == requestID).ToList();

                    List<Attachment> Attachments = new List<Attachment>();

                    foreach (var attachment in attachments)
                    {

                   
                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = requestID;
                        attach.Type = typeof(Request).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                        }
                        if (attachmentstodelete != null && attachmentstodelete.Count() > 0)
                        {
                            attachmentstodelete = attachmentstodelete.Where(w => !Attachments.Select(s => s.ID).Contains(w.ID)).ToList();
                            foreach (var att in attachmentstodelete)
                            {
                                if (att != null)
                                {
                                    Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                                }
                            }
                        }
                    }
                    result = dataAccess.Complete();
                }

            }
            return result > 0;
        }
        public bool DeleteOldRequestAttachment(int requestID)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.GetAll().ToList();
                var attachmentstodelete = SourceAttachments.Where(w => w.RelatedItemID == requestID).ToList();

                if (attachmentstodelete != null && attachmentstodelete.Count() > 0)
                {
                    foreach (var att in attachmentstodelete)
                    {
                        if (att != null)
                        {
                            Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                        }
                    }
                }
                result = dataAccess.Complete();

            }
            return result > 0;
        }
        #endregion

        #region Delete

        public bool Delete(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                Attachment Attachment = dataAccess.Attachment.Query().Include(a => a.Content).Where(w => w.ID == id).FirstOrDefault();
                if (Attachment != null)
                {
                    dataAccess.Attachment.Delete(Attachment);
                    result = dataAccess.Complete();
                    dataAccess.Dispose();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> DeleteAttachment(List<Attachment> Attachment)
        {
            int result = 0;

            using (var dataAccess = _factory.Create())
            {
                List<Attachment> Attachments = await dataAccess.Attachment.Query().IncludeOptimized(a => a.Content).ToListAsync();
                Attachments = Attachments.Where(s => Attachment.Select(x => x.ID).Contains(s.ID)).ToList();
                foreach (var attach in Attachments)
                {
                    dataAccess.Attachment.Delete(attach);
                    result = dataAccess.Complete();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }
        #endregion
    }
}
