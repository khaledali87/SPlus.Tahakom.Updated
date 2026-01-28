using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SPlus.Model;
using SPlus.API.Attribute;
using System.Web.Http.Cors;
using SPlus.DTO;
using SPlus.UseCases;
using StructureMap;
using SPlus.Helper;
using SPlus.Model.Domain;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HandshakeController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly HandshakeUseCases HandshakeUseCases;
        public HandshakeController()
        {
            HandshakeUseCases = _Container.GetInstance<HandshakeUseCases>();
        }

        [OperationContract]  [DataFormatingInvoker]
        //[BasicAuthenticationInvoker(CheckSecurityToken = false)]
        [HttpGet]
        public ResultWrapper<List<LookupDTO>> getHandshake()
        {
            ResultWrapper<List<LookupDTO>> result = new ResultWrapper<List<LookupDTO>>();
            try
            {
                var res = HandshakeUseCases.Read();
                List<LookupDTO> lookups = AutoMapper.Mapper.Map<List<LookupDTO>>(res);
                result.Data = lookups;
            }
            catch (System.Exception ex)
            {
                result.Data = null;
                result.ErrorCode = "0000";
                result.StatusCode = "fail";
                if (Constants._Error)
                    result.StatusMessage = ex.Message;
                else
                    result.StatusCode = "An error has occured";
            }

            return result;
        }

        
    }
}
