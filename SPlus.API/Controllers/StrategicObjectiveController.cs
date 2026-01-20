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
    public class StrategicObjectiveController : ApiController
    {


        Container _Container = IOC.InitializeContainer();
        private readonly StrategicObjectiveUseCases StrategicObjectiveUseCases;
        public StrategicObjectiveController()
        {
            StrategicObjectiveUseCases = _Container.GetInstance<StrategicObjectiveUseCases>();
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<List<CAStrategicObjectiveDTO>> Read()
        {
            ResultWrapper<List<CAStrategicObjectiveDTO>> result = new ResultWrapper<List<CAStrategicObjectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategicObjectiveUseCases.Read();
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<CAStrategicObjectiveDTO> Read(int id)
        {
            ResultWrapper<CAStrategicObjectiveDTO> result = new ResultWrapper<CAStrategicObjectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategicObjectiveUseCases.ReadByID(id);
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Theme/StrategicObjective/KPIs/{themeID}")]
        public ResultWrapper<List<CAStrategicObjectiveKPIsDTO>> StrategicObjectiveKPIs(int themeID, int? Year = null)
        {
            ResultWrapper<List<CAStrategicObjectiveKPIsDTO>> result = new ResultWrapper<List<CAStrategicObjectiveKPIsDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategicObjectiveUseCases.ReadWithStrategicalKPIsByThemeID(themeID, userName, Year);
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("StrategicObjectives/DivisionalObjectives")]
        public ResultWrapper<List<StrategicObjectiveChildDTO>> ReadWithDivisional()

        {
            ResultWrapper<List<StrategicObjectiveChildDTO>> result = new ResultWrapper<List<StrategicObjectiveChildDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategicObjectiveUseCases.ReadWithDivisional();
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("StrategicObjective/KPIs")]
        public ResultWrapper<List<SimpleStrategicObjectiveDTO>> StrategicObjectiveKPIs(int? Year = null)
        {
            ResultWrapper<List<SimpleStrategicObjectiveDTO>> result = new ResultWrapper<List<SimpleStrategicObjectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategicObjectiveUseCases.StrategicObjectiveKPIs(userName, Year);
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Report/StrategicObjective")]
        public ResultWrapper<List<CAStrategicObjectiveDTO>> ReportStrategicObjectives(string filterDate = null)
        {
            ResultWrapper<List<CAStrategicObjectiveDTO>> result = new ResultWrapper<List<CAStrategicObjectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategicObjectiveUseCases.ReportStrategicObjectives(filterDate);
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("StrategicObjective/ObjectiveDetails/{id}")]
        public ResultWrapper<StrategicObjectiveWithCardDTO> ReadStrategicObjectiveDetails(int id, int? Year = null)
        {
            ResultWrapper<StrategicObjectiveWithCardDTO> result = new ResultWrapper<StrategicObjectiveWithCardDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategicObjectiveUseCases.StrategicObjectiveDetails(id, userName, Year);
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        public ResultWrapper<CAStrategicObjectiveDTO> Create(CAStrategicObjectiveDTO StrategicObjectiveDTO)
        {
            ResultWrapper<CAStrategicObjectiveDTO> result = new ResultWrapper<CAStrategicObjectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = StrategicObjectiveUseCases.Create(StrategicObjectiveDTO);
                    if (result.Data.ID == -1 && result.Data.EnglishName == "Order Must be Unique")
                    {
                        result.Data = null;
                        result.ErrorCode = "0000";
                        result.StatusCode = "fail";
                        result.StatusMessage = "Order Must be Unique";
                    }
                    Task.Run(async () => { await StrategicObjectiveUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });

                }
                catch (DbEntityValidationException ex)
                {
                    // Handle validation exception
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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<CAStrategicObjectiveDTO> Update(CAStrategicObjectiveDTO StrategicObjectiveDTO)
        {
            ResultWrapper<CAStrategicObjectiveDTO> result = new ResultWrapper<CAStrategicObjectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var strategicObjective = StrategicObjectiveUseCases.Update(StrategicObjectiveDTO);

                    result.Data = strategicObjective;
                    if (result.Data.ID == -1 && result.Data.EnglishName == "Order Must be Unique")
                    {
                        result.Data = null;
                        result.ErrorCode = "0000";
                        result.StatusCode = "fail";
                        result.StatusMessage = "Order Must be Unique";
                    }
                    Task.Run(async () => { await StrategicObjectiveUseCases.Log(Token, strategicObjective, AuditTrailActionENums.Update); });

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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                    bool res = StrategicObjectiveUseCases.Delete(id);
                    result.Data = res;

                    Task.Run(async () => { await StrategicObjectiveUseCases.Log(Token, "Strategic Objective", AuditTrailActionENums.Delete, id); });

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
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategicObjectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

    }
}
