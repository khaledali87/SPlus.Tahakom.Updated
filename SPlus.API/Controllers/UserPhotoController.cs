using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SPlus.UseCases;
using SPlus.API.Attribute;
using System.Web.Http.Cors;
using StructureMap;
using SPlus.Helper;

namespace SPlus.API.Controllers
{
   [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserPhotoController : ApiController
    {
        Container _Container = IOC.InitializeContainer();
        private readonly UserUseCases UserUseCases;
        public UserPhotoController()
        {
            UserUseCases = _Container.GetInstance<UserUseCases>();
        }

        
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public IHttpActionResult GetUserPhoto()
        {
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                HttpResponseMessage result;
                //IHttpActionResult final;

                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result = UserUseCases.GetUserPhoto(userName);
                    
                }
                catch (Exception ex)
                {
                    UserUseCases.CreateException("Exception", ex.Message, "UserPhoto", "GetUserPhoto");
                    result = null;

                }
                return this.ResponseMessage(result);
            }
            else
                //    { return null; }

                return null;
        }
        
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("UserPhoto/{UserName}")]
        public HttpResponseMessage GetUserPhotobyUserName(string UserName)
        {

            if (UserName != null)
            {
                HttpContent content;
                HttpResponseMessage result = null;

                try
                {
                   result = UserUseCases.GetUserPhoto(UserName);
                }
                catch (Exception ex)
                {
                    UserUseCases.CreateException("Exception", ex.Message, "UserPhoto", "GetUserPhotobyUserName");
                    result = null;

                }
                
                return result;
            }
            else              
                return null;
        }
    }
}
