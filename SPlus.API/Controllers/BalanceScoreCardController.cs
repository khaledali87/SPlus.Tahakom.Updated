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
using SPlus.Helper;
using System.Threading.Tasks;
using StructureMap;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BalanceScoreCardController : ApiController
    {
        Container _Container = IOC.InitializeContainer();
        private readonly BalanceScoreCardUseCases BalanceScoreCardUseCases;

        public BalanceScoreCardController()
        {
            BalanceScoreCardUseCases = _Container.GetInstance<BalanceScoreCardUseCases>();

        }
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("BalanceScoreCard/Operational")]
        public ResultWrapper<OperationalBalanceScoreCardOrgStructureDTO> ReadOperational(int? Year = null)
        {
            ResultWrapper<OperationalBalanceScoreCardOrgStructureDTO> result = new ResultWrapper<OperationalBalanceScoreCardOrgStructureDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = BalanceScoreCardUseCases.GetOperationalBalanceScoreCard(userName, Year);
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
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("BalanceScoreCard/Operational/{depratmentId}")]
        public ResultWrapper<BalanceScoreDepratmentDetailsDTO> ReadOperationalByDepartmentId(int depratmentId, [FromBody] List<int> months, int? Year = null)
        {
            ResultWrapper<BalanceScoreDepratmentDetailsDTO> result = new ResultWrapper<BalanceScoreDepratmentDetailsDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = BalanceScoreCardUseCases.ReadOperationalByDepartmentId(userName, depratmentId, months, Year);
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
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }



         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("BalanceScoreCard/ReadCorporate")]
        public ResultWrapper<BalanceScoreDepratmentDetailsDTO> ReadCorporate(CoparateSectorFilterDTO filter, int? Year = null)
        {
            ResultWrapper<BalanceScoreDepratmentDetailsDTO> result = new ResultWrapper<BalanceScoreDepratmentDetailsDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = BalanceScoreCardUseCases.ReadCorporate(userName, filter, Year);
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
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        BalanceScoreCardUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }




    }
}
