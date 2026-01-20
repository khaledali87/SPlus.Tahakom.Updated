using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SPlus.Model;
using SPlus.Helper;
using SPlus.API.Attribute;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using System.Text;
using SPlus.DTO;
using SPlus.UseCases;
using SPlus.Model.Domain;
using StructureMap;
using System.Net.Http.Headers;
using System.IO;
using System.Web.Hosting;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AttachmentController : ApiController
    {
        Container _Container = IOC.InitializeContainer();
        private readonly AttachmentUseCases AttachementUseCases;
        public AttachmentController()
        {
            AttachementUseCases = _Container.GetInstance<AttachmentUseCases>();
        }
        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]

        [Route("Attachment/{id}")]
        public HttpResponseMessage Read(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                result = AttachementUseCases.GetFile(id);

                result.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = new TimeSpan(1, 0, 0, 0)
                };

            }
            catch (System.Exception ex)
            {
                if (ex.InnerException == null)
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Attachment")]
        public ResultWrapper<List<AttachmentDTO>> ReadAttachments()
        {
            ResultWrapper<List<AttachmentDTO>> result = new ResultWrapper<List<AttachmentDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = AttachementUseCases.Read();
                    result.Data = res;
                }
                catch (System.Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An Error has Occured";
                    if (ex.InnerException == null)
                        AttachementUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Attachment/KPI/{KPIID}")]
        public ResultWrapper<List<KPIAttachmentDTO>> ReadAttachmentByKPIID(int KPIID)
        {
            ResultWrapper<List<KPIAttachmentDTO>> result = new ResultWrapper<List<KPIAttachmentDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = AttachementUseCases.ReadByKPIID(KPIID, userName);
                    result.Data = res;
                }
                catch (System.Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An Error has Occured";
                    if (ex.InnerException == null)
                        AttachementUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name); result = null;
                }
            }
            return result;
        }



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("Attachment")]
        public async Task<ResultWrapper<List<int>>> UploadFile(HttpRequestMessage request)
        {
            ResultWrapper<List<int>> result = new ResultWrapper<List<int>>();
            try
             {
                //var provider = await Request.Content.ReadAsMultipartAsync();
                //var fileContent = provider.Contents.FirstOrDefault();
                //if (fileContent == null)
                //{
                //    return null;
                //}

                //// Get file name
                //var fileName = fileContent.Headers.ContentDisposition.FileName?.Trim('"');

                //// Read bytes into memory
                //var fileBytes = await fileContent.ReadAsByteArrayAsync();

                //using (var fs = new FileStream(HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}/files/{fileName}")), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                //{
                //    await fs.WriteAsync(fileBytes, 0, fileBytes.Length).ConfigureAwait(false);
                //}



                //// Save to temp file (because iText needs a file path or stream)
                //var tempPath = Path.Combine(Path.GetTempPath(), HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}/files/{fileName}")));
                //// await File.WriteAllBytesAsync(tempPath, fileBytes);

                //// Scan the PDF
                //var scanner = new PdfSecurityScanner();




                //var result1 = scanner.ScanPdfFile(tempPath);



                if (request.Content.IsMimeMultipartContent())
                {
                    var data = await ParseMultipartAsync(Request.Content);

                    if (data.Files.Count > 0)
                    {

                        result = AttachementUseCases.UploadAttachment(data);

                    }
                    else
                    {
                        result.StatusMessage = "No Attachment Uploaded";
                        return result;

                    }
                    return result;
                }
                else
                {
                    result.StatusMessage = "No Attachment Uploaded";
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException == null)
                {
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    result.StatusMessage = ex.Message;
                }
                else if (ex.InnerException.InnerException == null)
                {
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    result.StatusMessage = ex.InnerException.Message;
                }
                else
                {
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    result.StatusMessage = ex.InnerException.InnerException.Message;
                }
                return result;

            }
        }

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpDelete]
        [Route("Attachment/{id}")]
        public ResultWrapper<bool> Delete(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            try
            {
                result.Data = AttachementUseCases.Delete(id);
            }
            catch (System.Exception ex)
            {
                result.Data = false;
                result.ErrorCode = "0000";
                result.StatusCode = "fail";
                if (Constants._Error)
                    result.StatusMessage = ex.Message;
                else
                    result.StatusCode = "An Error has Occured";
                if (ex.InnerException == null)
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    AttachementUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return result;
            }
            return result;
        }


        #region ParseMultipartAsync
        private async Task<HttpPostedData> ParseMultipartAsync(HttpContent postedContent)
        {
            var provider = await postedContent.ReadAsMultipartAsync();
            PdfSecurityScanResult checkFileResult = null;

            bool isSafe = true;
            var fileContent = provider.Contents.FirstOrDefault();
            if (fileContent == null)
            {
                return null;
            }

            // Get file name
            var createdFileName = fileContent.Headers.ContentDisposition.FileName?.Trim('"');
            if (Path.GetExtension(createdFileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                // Read bytes into memory
                var fileBytes = await fileContent.ReadAsByteArrayAsync();
                using (var fs = new FileStream(HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}/files/{createdFileName}")), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await fs.WriteAsync(fileBytes, 0, fileBytes.Length).ConfigureAwait(false);
                }
                // Save to temp file (because iText needs a file path or stream)
                var tempPath = Path.Combine(Path.GetTempPath(), HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}/files/{createdFileName}")));
                // await File.WriteAllBytesAsync(tempPath, fileBytes);
                // Scan the PDF
                var scanner = new PdfSecurityScanner();
                checkFileResult = scanner.ScanPdfFile(tempPath);
                isSafe = checkFileResult.IsSafe;
                if (File.Exists(HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}/files/{createdFileName}"))))  // always check first to avoid exceptions
                {
                    File.Delete(HostingEnvironment.MapPath(Path.Combine($"{Constants._LogPath}/files/{createdFileName}")));
                   
                }
            }
            var files = new Dictionary<string, HttpPostedFile>(StringComparer.InvariantCultureIgnoreCase);
            var fields = new Dictionary<string, HttpPostedField>(StringComparer.InvariantCultureIgnoreCase);
            int SameFileCount = 0;
            foreach (var content in provider.Contents)
            {
                var fieldName = content.Headers.ContentDisposition.Name.Trim('"') + content.Headers.ContentDisposition.FileName.Trim('"');
                if (!string.IsNullOrWhiteSpace(content.Headers.ContentDisposition.FileName))
                {
                    var file = await content.ReadAsByteArrayAsync();
                    var fileName = content.Headers.ContentDisposition.FileName.Trim('"');
                    SameFileCount = files.Where(a => a.Key == "file" + fileName).Count() + SameFileCount;
                    if (SameFileCount >= 1)
                    {
                        string Name = fileName.Remove(fileName.LastIndexOf('.'));
                        string ext = fileName.Remove(0, fileName.LastIndexOf('.'));
                        fileName = Name + "_" + SameFileCount + ext;
                        fieldName = "file" + fileName;
                    }
                    bool allZero = file.All(b => b == 0);
                    if (isSafe&& !allZero)
                        files.Add(fieldName, new HttpPostedFile(fieldName, fileName, file));
                }
                else
                {
                    var data = await content.ReadAsStringAsync();
                   
                    if (isSafe)
                        fields.Add(fieldName, new HttpPostedField(fieldName, data));
                }
            }

            return new HttpPostedData(fields, files);
        }
        #endregion

    }
}
