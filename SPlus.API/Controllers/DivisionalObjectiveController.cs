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
using System.Threading.Tasks;
using SPlus.Helper;
using StructureMap;
using AutoMapper;
using Microsoft.SqlServer.Server;
using System.Data.Entity.Validation;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DivisionalObjectiveController : ApiController
    {
        Container _Container = IOC.InitializeContainer();
        private readonly DivisionalObjectiveUseCases DivisionalObjectiveUseCases;
        public DivisionalObjectiveController()
        {
            DivisionalObjectiveUseCases = _Container.GetInstance<DivisionalObjectiveUseCases>();
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        public ResultWrapper<CADivisionalObjectiveDTO> Create(CADivisionalObjectiveDTO divisionalObjective)
        {
            ResultWrapper<CADivisionalObjectiveDTO> result = new ResultWrapper<CADivisionalObjectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = DivisionalObjectiveUseCases.Create(divisionalObjective);
                    if (result.Data.ID == -1 && result.Data.EnglishName == "Order Must be Unique")
                    {
                        result.Data = null;
                        result.ErrorCode = "0000";
                        result.StatusCode = "fail";
                        result.StatusMessage = "Order Must be Unique";
                    }
                    Task.Run(async () => { await DivisionalObjectiveUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });

                }
                catch (DbEntityValidationException ex)
                {

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
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<List<CADivisionalObjectiveDTO>> Read()
        {
            ResultWrapper<List<CADivisionalObjectiveDTO>> result = new ResultWrapper<List<CADivisionalObjectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = DivisionalObjectiveUseCases.Read();
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
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
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("DivisionalObjective/KPIs/{id}")]
        public ResultWrapper<List<CAODivisionalObjectiveKPIDTO>> ReadByStrategicObjectivesIDWithKPIs(int id, int? Year = null)
        {
            ResultWrapper<List<CAODivisionalObjectiveKPIDTO>> result = new ResultWrapper<List<CAODivisionalObjectiveKPIDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = DivisionalObjectiveUseCases.ReadByStrategicObjectivesIDWithKPIs(id, Year);
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
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
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<CADivisionalObjectiveDTO> Read(int id)
        {
            ResultWrapper<CADivisionalObjectiveDTO> result = new ResultWrapper<CADivisionalObjectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = DivisionalObjectiveUseCases.ReadByID(id, userName);
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
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
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("DivisionalObjective/ObjectiveDetails/{id}")]
        public ResultWrapper<DivisionalObjectiveWithCardDTO> ReadDivisionalObjectiveDetails(int id, int? Year = null)
        {
            ResultWrapper<DivisionalObjectiveWithCardDTO> result = new ResultWrapper<DivisionalObjectiveWithCardDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = DivisionalObjectiveUseCases.DivisionalObjectiveDetails(id, userName, Year);
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
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
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Report/DivisionalObjective")]
        public ResultWrapper<List<CADivisionalObjectiveDTO>> ReportDivisionalObjective(string filterDate = null)
        {
            ResultWrapper<List<CADivisionalObjectiveDTO>> result = new ResultWrapper<List<CADivisionalObjectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = DivisionalObjectiveUseCases.ReportDivisionalObjective(userName, filterDate);
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
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
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<CADivisionalObjectiveDTO> Update(CADivisionalObjectiveDTO divisionalObjectiveDTO)
        {
            ResultWrapper<CADivisionalObjectiveDTO> result = new ResultWrapper<CADivisionalObjectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var divisionalObjective = DivisionalObjectiveUseCases.Update(divisionalObjectiveDTO);

                    result.Data = divisionalObjective;
                    if (result.Data.ID == -1 && result.Data.EnglishName == "Order Must be Unique")
                    {
                        result.Data = null;
                        result.ErrorCode = "0000";
                        result.StatusCode = "fail";
                        result.StatusMessage = "Order Must be Unique";
                    }
                    Task.Run(async () => { await DivisionalObjectiveUseCases.Log(Token, divisionalObjective, AuditTrailActionENums.Update); });

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
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

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
                    bool res = DivisionalObjectiveUseCases.Delete(id);
                    result.Data = res;

                    Task.Run(async () => { await DivisionalObjectiveUseCases.Log(Token, "Strategic Objective", AuditTrailActionENums.Delete, id); });

                }
                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DivisionalObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

    }
}
