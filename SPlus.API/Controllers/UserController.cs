using SPlus.API.Attribute;
using SPlus.UseCases;
using SPlus.DTO;
using SPlus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using StructureMap;
using SPlus.Helper;
using System.Data.Entity.Validation;
using System.Threading.Tasks;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly UserUseCases UserUseCases;
        public UserController()
        {
            UserUseCases = _Container.GetInstance<UserUseCases>();
        }
        #region New


        #region Users
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("User")]
        public ResultWrapper<List<UserListDTO>> GetUsers()
        {
            ResultWrapper<List<UserListDTO>> result = new ResultWrapper<List<UserListDTO>>();
            try
            {
                var res = UserUseCases.Read();
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
                    UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("User/Champion")]
        public ResultWrapper<List<UserListDTO>> GetChampionUsers()
        {
            ResultWrapper<List<UserListDTO>> result = new ResultWrapper<List<UserListDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = UserUseCases.GetChampionUsers();
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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("ByUser/{username?}")]
        public ResultWrapper<UserDTO> GetUser(string username)
        {
            ResultWrapper<UserDTO> result = new ResultWrapper<UserDTO>();
            try
            {
                if (!string.IsNullOrWhiteSpace(username))
                {
                    var res = UserUseCases.ReadByUserName(username);
                    result.Data = res;
                }
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
                    UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("User/Validate")]
        public ResultWrapper<List<UserDTO>> ValidateUser(UserDTO user)
        {
            ResultWrapper<List<UserDTO>> result = new ResultWrapper<List<UserDTO>>();
            try
            {
                var res = UserUseCases.Validate(user.UserName);
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
                    UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("User/Replace")]
        public ResultWrapper<bool> ReplaceUser(ReplaceUserDTO replaceUser)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = UserUseCases.ReplaceUser(replaceUser);
                    Task.Run(async () => { await UserUseCases.Log(Token, replaceUser, AuditTrailActionENums.Update); });
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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("User")]
        public ResultWrapper<UserDTO> Create(UserDTO user)
        {
            ResultWrapper<UserDTO> result = new ResultWrapper<UserDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = UserUseCases.Create(user);
                    Task.Run(async () => { await UserUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });
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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("User")]
        public ResultWrapper<UserDTO> Update(UserDTO user)
        {
            ResultWrapper<UserDTO> result = new ResultWrapper<UserDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = UserUseCases.Update(user);
                    Task.Run(async () => { await UserUseCases.Log(Token, user, AuditTrailActionENums.Update); });

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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


        #endregion

        #region Groups

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Group")]
        public ResultWrapper<List<GroupListDTO>> GetGroups()
        {
            ResultWrapper<List<GroupListDTO>> result = new ResultWrapper<List<GroupListDTO>>();
            try
            {
                var res = UserUseCases.ReadGroup();
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
                    UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Group/GroupsByUser/{user}")]
        public ResultWrapper<List<SingularGroupDTO>> GetGroup(string user)
        {
            ResultWrapper<List<SingularGroupDTO>> result = new ResultWrapper<List<SingularGroupDTO>>();
            try
            {
                var res = UserUseCases.ReadGroupByUserName(user);
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
                    UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }



         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Group/UserGroups")]
        public ResultWrapper<List<SingularGroupDTO>> GetUserGroupsByToken()
        {
            ResultWrapper<List<SingularGroupDTO>> result = new ResultWrapper<List<SingularGroupDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                try
                {
                    string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                    var res = UserUseCases.ReadGroupByUserName(userName);
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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Group/{id}")]
        public ResultWrapper<SingularGroupDTO> GetGroupByID(int id)
        {
            ResultWrapper<SingularGroupDTO> result = new ResultWrapper<SingularGroupDTO>();
            try
            {
                var res = UserUseCases.ReadGroupByID(id);
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
                    UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("Group")]
        public ResultWrapper<GroupListDTO> CreateGroup(SingularGroupDTO group)
        {
            ResultWrapper<GroupListDTO> result = new ResultWrapper<GroupListDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = UserUseCases.CreateGroup(group);
                    Task.Run(async () => { await UserUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });
                }
                catch (DbEntityValidationException ex)
                {
                    Exception raise = ex;
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    if (ex.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    throw raise;
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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("Group")]
        public ResultWrapper<GroupListDTO> Update(SingularGroupDTO group)
        {
            ResultWrapper<GroupListDTO> result = new ResultWrapper<GroupListDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = UserUseCases.Update(group);
                    Task.Run(async () => { await UserUseCases.Log(Token, group, AuditTrailActionENums.Update); });
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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpDelete]
        [Route("Group/{id}")]
        public ResultWrapper<bool> DeleteGroup(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = UserUseCases.DeleteGroup(id);
                    Task.Run(async () => { await UserUseCases.Log(Token, "Group", AuditTrailActionENums.Delete, id); });

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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpDelete]
        [Route("User")]
        public ResultWrapper<bool> DeleteUser([FromBody] UserDTO user)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = UserUseCases.Delete(user.UserName);
                    Task.Run(async () => { await UserUseCases.Log(Token, "user", AuditTrailActionENums.Delete); });

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
                        UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
        #endregion


        #region Resources
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Resources")]
        public ResultWrapper<List<ResourceDTO>> ReadResources()
        {
            ResultWrapper<List<ResourceDTO>> result = new ResultWrapper<List<ResourceDTO>>();
            try
            {
                result.Data = UserUseCases.ReadResources();
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
                    UserUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    UserUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        #endregion

        #endregion






    }
}