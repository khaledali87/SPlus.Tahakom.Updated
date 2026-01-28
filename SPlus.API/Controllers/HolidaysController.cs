using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Http;
using SPlus.Model;
using SPlus.API.Attribute;
using System.Web.Http.Cors;
using SPlus.UseCases;
using StructureMap;
using SPlus.Helper;
using SPlus.Model.Domain;
using SPlus.DTO;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HolidaysController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly HolidayUseCases HolidayUseCases;
        public HolidaysController()
        {
            HolidayUseCases = _Container.GetInstance<HolidayUseCases>();
        }

        [OperationContract]
        [DataFormatingInvoker]
        //[BasicAuthenticationInvoker(CheckSecurityToken = false)]
        [HttpGet]
        public ResultWrapper<List<Holiday>> GetHolidays()
        {
            ResultWrapper<List<Holiday>> result = new ResultWrapper<List<Holiday>>();
            try
            {
                result.Data = HolidayUseCases.Read();
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


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        public ResultWrapper<Holiday> CreateHoliday(Holiday Holiday)
        {
            ResultWrapper<Holiday> result = new ResultWrapper<Holiday>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());

                try
                {
                    var res = HolidayUseCases.Create(Holiday);
                    result.Data = res;
                    Task.Run(async () => { await HolidayUseCases.Log(Token, res, AuditTrailActionENums.Create); });

                }
                catch (DbEntityValidationException ex)
                {
                    System.Exception raise = ex;
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
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    throw raise;
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

                    if (ex.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<Holiday> UpdateHoliday(Holiday Holiday)
        {
            ResultWrapper<Holiday> result = new ResultWrapper<Holiday>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = HolidayUseCases.Update(Holiday);
                    Task.Run(async () => { await HolidayUseCases.Log(Token, Holiday, AuditTrailActionENums.Update); });

                }
                catch (DbEntityValidationException ex)
                {
                    System.Exception raise = ex;
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
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);

                    throw raise;
                }
                catch (System.Exception ex)
                {
                    result.Data = null;// new KPI();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<bool> UpdateHolidayStatus(int id , bool status)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = HolidayUseCases.UpdateStatus(id,status);
                    Task.Run(async () => { await HolidayUseCases.Log(Token, status, AuditTrailActionENums.Update); });

                }
                catch (DbEntityValidationException ex)
                {
                    System.Exception raise = ex;
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
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);

                    throw raise;
                }
                catch (System.Exception ex)
                {
                    result.Data = default;// new KPI();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        HolidayUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpDelete]
        public ResultWrapper<bool> DeleteHoliday(int ID)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                try
                {
                    result.Data = HolidayUseCases.Delete(ID);
                    Task.Run(async () => { await HolidayUseCases.Log(Token, "Holiday", AuditTrailActionENums.Delete); });

                }
                catch (System.Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    HolidayUseCases.CreateException("Exception", ex.Message, "KPI", "ReadTypes");
                }
            }
            return result;
        }
    }
}
