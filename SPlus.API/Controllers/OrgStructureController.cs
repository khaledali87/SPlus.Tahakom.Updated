using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SPlus.Model;
using SPlus.UseCases;
using SPlus.API.Attribute;
using System.Web.Http.Cors;
using SPlus.DTO;
using StructureMap;
using SPlus.Helper;
using System.Threading.Tasks;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrgStructureController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly OrgStructureUseCases OrgStructureUseCases;
        public OrgStructureController()
        {
            OrgStructureUseCases = _Container.GetInstance<OrgStructureUseCases>();
        }

        #region Create 
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        public ResultWrapper<OrgStructureDTO> Create(OrgStructureDTO OrgStructureDTO)
        {
            ResultWrapper<OrgStructureDTO> result = new ResultWrapper<OrgStructureDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = OrgStructureUseCases.Create(OrgStructureDTO, userName);
                    Task.Run(async () => { await OrgStructureUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });

                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Read
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<List<OrgStructureDTO>> Read(int? Year = null)
        {
            ResultWrapper<List<OrgStructureDTO>> result = new ResultWrapper<List<OrgStructureDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = OrgStructureUseCases.Read(userName, Year);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("OrgStructure/AssignedUser")]
        public ResultWrapper<List<OrgStructureDTO>> ReadByManger(int? Year = null)
        {
            ResultWrapper<List<OrgStructureDTO>> result = new ResultWrapper<List<OrgStructureDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = OrgStructureUseCases.ReadReadByManger(userName, Year);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("OrgStructure/KPIs/{orgID}")]
        public ResultWrapper<List<CADepartmentKPIsDTO>> ReadCA(int orgID)
        {
            ResultWrapper<List<CADepartmentKPIsDTO>> result = new ResultWrapper<List<CADepartmentKPIsDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = OrgStructureUseCases.ReadCA(orgID, userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("OrgStructure/ReadFlat")]
        public ResultWrapper<List<OrgStructureDTO>> ReadFlat(int? Year = null)
        {
            ResultWrapper<List<OrgStructureDTO>> result = new ResultWrapper<List<OrgStructureDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = OrgStructureUseCases.Read(userName, Year);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<OrgStructureDTO> ReadByID(int id)
        {
            ResultWrapper<OrgStructureDTO> result = new ResultWrapper<OrgStructureDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = OrgStructureUseCases.ReadByID(id, userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("OrgStructureMap")]
        public ResultWrapper<List<OrgStructureMapDTO>> ReadOrgStructureMap(int? Year = null)
        {
            ResultWrapper<List<OrgStructureMapDTO>> result = new ResultWrapper<List<OrgStructureMapDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = OrgStructureUseCases.OrgStructureMap(userName, Year);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Update
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<OrgStructureDTO> Update(OrgStructureDTO OrgStructureDTO)
        {
            ResultWrapper<OrgStructureDTO> result = new ResultWrapper<OrgStructureDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = OrgStructureUseCases.Update(OrgStructureDTO, userName);
                    Task.Run(async () => { await OrgStructureUseCases.Log(Token, OrgStructureDTO, AuditTrailActionENums.Update); });
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Delete
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpDelete]
        public ResultWrapper<bool> Delete(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = OrgStructureUseCases.Delete(id);
                    Task.Run(async () => { await OrgStructureUseCases.Log(Token, "OrgStructure", AuditTrailActionENums.Delete); });

                }
                catch (Exception ex)
                {
                    result.Data = false;// new bool();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        OrgStructureUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

    }
}
