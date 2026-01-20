using SPlus.Helper;
using SPlus.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using System.Reflection;
using System.Net.Http;
using SPlus.Model;

namespace SPlus.PPlusADO
{
    public class PPlusServices
    {
        private static string token;
        private static string URL;
        private static string basicToken;
        private static ServiceLogHelper log;
        public PPlusServices()
        {
            log = new ServiceLogHelper();
            URL = Constants.PPlusURL;
            basicToken = Constants.PPlusBasicToken;
            token = Constants.PPlusToken;
        }

        public List<StrategicInitiative> GetStrategicInitiative()
        {
            string ServiceURL = URL + "projectservice.svc/Programs";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            try
            {
                APIResponse<List<StrategicInitiative>> res = new APIResponse<List<StrategicInitiative>>();
                var client = new RestClient(ServiceURL);
                var request = new RestRequest(ServiceURL, Method.Get);
                request.Timeout = -1;
                request.AddHeader("Authorization", basicToken);
                request.AddHeader("Token", token);
                //request.AddHeader("Content-Type", "application/json");
                watch.Reset();
                watch.Start();
                var response = client.Execute<APIResponse<List<StrategicInitiative>>>(request);
                watch.Stop();
                if (response.StatusCode.ToString() == "OK")
                {
                    res = JsonConvert.DeserializeObject<APIResponse<List<StrategicInitiative>>>(response.Content);
                    return res.Data;
                }
                else
                {
                    Task.Run(() =>
                    {
                        log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", response.StatusCode.ToString());
                    });
                    return new List<StrategicInitiative>();
                }
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException == null)
                    log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", ex.Message);

                else if (ex.InnerException.InnerException == null)
                    log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", ex.InnerException.Message);

                else
                    log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", ex.InnerException.InnerException.Message);

                return new List<StrategicInitiative>();
            }
        }

        public CreateProgramResult CreateStrategicInitiative(StrategicInitiative strategicInitiative)
        {
            string ServiceURL = URL + "API.svc/Program/Create";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            try
            {
                CreateProgramResponse res = new CreateProgramResponse();
                var client = new RestClient(ServiceURL);
                var request = new RestRequest(ServiceURL, Method.Post);
                request.Timeout = -1;
                request.AddHeader("Authorization", basicToken);
                request.AddHeader("Token", token);
                request.AddHeader("Content-Type", "application/json");

                strategicInitiative.ID = 0;
                var formData = new
                {
                    formData = new
                    {
                        ProjectUID = strategicInitiative.ProjectUID.ToString(),
                        NameEnglish = strategicInitiative.EnglishName,
                        NameArabic = strategicInitiative.ArabicName,
                        ArabicDescription = strategicInitiative.ArabicDescription,
                        EnglishDescription = strategicInitiative.EnglishDescription,
                        Manager = strategicInitiative.Manager,
                        StartDateValue = strategicInitiative.StartDate.ToString("dd-MM-yyyy"),
                        FinishDateValue = strategicInitiative.EndDate.ToString("dd-MM-yyyy"),
                    }
                };
                var body = JsonConvert.SerializeObject(formData);
                request.AddJsonBody(body);
                var responseasync  = client.PostAsync(request);
                var response = responseasync.Result;
                if (response.StatusCode.ToString() == "OK")
                {
                    res = JsonConvert.DeserializeObject<CreateProgramResponse>(response.Content);
                    return res.CreateProgramResult;
                }
                else
                {
                    Task.Run(() =>
                    {
                        log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", response.StatusCode.ToString());
                    });
                    return new CreateProgramResult()
                    {
                        Data = false,
                        StatusMessage = response.StatusDescription,
                        StatusCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CreateProgramResult()
                {
                    Data = false,
                    StatusMessage = ex.Message,
                    StatusCode = "fail"
                };
            }
        }

        public ResultWrapper<bool> UpdateStrategicInitiative(StrategicInitiative strategicInitiative)
        {
            string ServiceURL = URL + "API.svc/Program/Update";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            try
            {
                ResultWrapper<bool> res = new ResultWrapper<bool>();
                var client = new RestClient(ServiceURL);
                var request = new RestRequest(ServiceURL, Method.Post);
                request.Timeout = -1;
                request.AddHeader("Authorization", basicToken);
                request.AddHeader("Token", token);
                request.AddHeader("Content-Type", "application/json");

                var formData = new
                {
                    formData = new
                    {
                        ProjectUID = strategicInitiative.ProjectUID.ToString(),
                        NameEnglish = strategicInitiative.EnglishName,
                        NameArabic = strategicInitiative.ArabicName,
                        ArabicDescription = strategicInitiative.ArabicDescription,
                        EnglishDescription = strategicInitiative.EnglishDescription,
                        Manager = strategicInitiative.Manager,
                        StartDateValue = strategicInitiative.StartDate.ToString("dd-MM-yyyy"),
                        FinishDateValue = strategicInitiative.EndDate.ToString("dd-MM-yyyy"),
                    }
                };
                var body = JsonConvert.SerializeObject(formData);
                request.AddJsonBody(body);
                var responseasync = client.PostAsync(request);
                var response = responseasync.Result;
                if (response.StatusCode.ToString() == "OK")
                {
                    res = JsonConvert.DeserializeObject<ResultWrapper<bool>>(response.Content);
                    return res;
                }
                else
                {
                    Task.Run(() =>
                    {
                        log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", response.StatusCode.ToString());
                    });
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = response.StatusDescription,
                        StatusCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException == null)
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.Message,
                        StatusCode = "fail"
                    };
                else if (ex.InnerException.InnerException == null)
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.InnerException.Message,
                        StatusCode = "fail"
                    };
                else
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.InnerException.InnerException.Message,
                        StatusCode = "fail"
                    };
            }
        }
        public ResultWrapper<bool> DeleteStrategicInitiative(Guid ProjectUID)
        {
            string ServiceURL = URL + $"API.svc/Program/{ProjectUID}/Delete";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            try
            {
                ResultWrapper<bool> res = new ResultWrapper<bool>();
                var client = new RestClient(ServiceURL);
                var request = new RestRequest(ServiceURL, Method.Post);
                request.Timeout = -1;
                request.AddHeader("Authorization", basicToken);
                request.AddHeader("Token", token);
                request.AddHeader("Content-Type", "application/json");


                var response = client.Post(request);
                if (response.StatusCode.ToString() == "OK")
                {
                    res = JsonConvert.DeserializeObject<ResultWrapper<bool>>(response.Content);
                    return res;
                }
                else
                {
                    Task.Run(() =>
                    {
                        log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", response.StatusCode.ToString());
                    });
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = response.StatusDescription,
                        StatusCode = response.StatusCode.ToString()
                    };

                }
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException == null)
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.Message,
                        StatusCode = "fail"
                    };
                else if (ex.InnerException.InnerException == null)
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.InnerException.Message,
                        StatusCode = "fail"
                    };
                else
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.InnerException.InnerException.Message,
                        StatusCode = "fail"
                    };
            }
        }

        public ResultWrapper<bool> ValidateStrategicInitiative(string name)
        {
            string ServiceURL = URL + $"API.svc/Program/ValidateName";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            try
            {
                ResultWrapper<bool> res = new ResultWrapper<bool>();
                var client = new RestClient(ServiceURL);
                var request = new RestRequest(ServiceURL, Method.Post);
                request.Timeout = -1;
                request.AddHeader("Authorization", basicToken);
                request.AddHeader("Token", token);
                request.AddHeader("Content-Type", "application/json");

                var formData = new
                {
                     programName = name
                };
                var body = JsonConvert.SerializeObject(formData);
                request.AddJsonBody(body);
                var response = client.Post(request);
                if (response.StatusCode.ToString() == "OK")
                {
                    res = JsonConvert.DeserializeObject<ResultWrapper<bool>>(response.Content);
                    return res;
                }
                else
                {
                    Task.Run(() =>
                    {
                        log.ServiceLog($"{MethodBase.GetCurrentMethod().Name} Exception \nService URL: {ServiceURL}", response.StatusCode.ToString());
                    });
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = response.StatusDescription,
                        StatusCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException == null)
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.Message,
                        StatusCode = "fail"
                    };
                else if (ex.InnerException.InnerException == null)
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.InnerException.Message,
                        StatusCode = "fail"
                    };
                else
                    return new ResultWrapper<bool>()
                    {
                        Data = false,
                        StatusMessage = ex.InnerException.InnerException.Message,
                        StatusCode = "fail"
                    };
            }
        }
    }
}
