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
using System.Threading.Tasks;
using SPlus.DTO;
using System.Data.Entity.Validation;
using StructureMap;
using SPlus.Helper;

namespace SPlus.API.Controllers
{
   [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SystemPerformanceThresholdController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly SystemPerformanceThresholdUseCases SystemPerformanceThresholdUseCases;
        public SystemPerformanceThresholdController()
        {
            SystemPerformanceThresholdUseCases = _Container.GetInstance<SystemPerformanceThresholdUseCases>();
        }

        #region Read

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<List<SystemPerformanceThresholdDTO>> ReadKPIPerformanceThreshold()
        {
            ResultWrapper<List<SystemPerformanceThresholdDTO>> result = new ResultWrapper<List<SystemPerformanceThresholdDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                try
                {
                    var res = SystemPerformanceThresholdUseCases.SystemPerformanceThreshold();
                    //var x = AutoMapper.Mapper.Map<KPITypeDTO>(res[0]);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new List<SystemPerformanceThresholdDTO>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("SystemPerformanceThreshold/NA")]
        public ResultWrapper<List<SystemPerformanceThresholdDTO>> ReadKPIPerformanceThresholdNA()
        {
            ResultWrapper<List<SystemPerformanceThresholdDTO>> result = new ResultWrapper<List<SystemPerformanceThresholdDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                try
                {
                    var res = SystemPerformanceThresholdUseCases.SystemPerformanceThresholdNA();
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new List<SystemPerformanceThresholdDTO>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Update

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<List<UpdateSystemPerformanceThresholdDTO>> UpdateKPIPerformanceThreshold(List<UpdateSystemPerformanceThresholdDTO> _KPIPerformanceThresholds)
        {
            ResultWrapper<List<UpdateSystemPerformanceThresholdDTO>> result = new ResultWrapper<List<UpdateSystemPerformanceThresholdDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = SystemPerformanceThresholdUseCases.UpdateSystemPerformanceThreshold(_KPIPerformanceThresholds);
                    Task.Run(async () => { await SystemPerformanceThresholdUseCases.Log(Token, _KPIPerformanceThresholds, AuditTrailActionENums.Update); });

                }
                catch (Exception ex)
                {
                    result.Data = new List<UpdateSystemPerformanceThresholdDTO>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        SystemPerformanceThresholdUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


        #endregion


    }
}
