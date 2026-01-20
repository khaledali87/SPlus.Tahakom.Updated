using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static SPlus.Integration.Enums;
using SPlus.Model;

namespace SPlus.Integration.IntegrationKitchen.AccessImplementations.WebLevel
{
    public class APIAccess : IAccess
    {
        string apiUrl = "";
        private string baseAddress;

        public APIAccess(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        private void SetupClient(HttpClient client, string methodName, string apiUrl, object content = null)
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<string> GetSingleItemRequest()
        {
            System.Net.Http.HttpResponseMessage httpResponseHeaders = null;
            //System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK;
            //string error = "";
            string result = null;

            try
            {
                using (var client = new HttpClient(new System.Net.Http.HttpClientHandler()))
                {
                    SetupClient(client, "GET", apiUrl);

                    httpResponseHeaders = await client.GetAsync(apiUrl).ConfigureAwait(false);

                    httpResponseHeaders.EnsureSuccessStatusCode();

                    await httpResponseHeaders.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                    {
                        if (x.IsFaulted)
                        {
                            //logging

                            //statusCode = System.Net.HttpStatusCode.InternalServerError;
                            //error = x.Exception.Message;
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<string>(x.Result);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                if (httpResponseHeaders != null)
                {
                    await httpResponseHeaders.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                    {
                        //logging

                        //statusCode = httpResponseHeaders.StatusCode;
                        //error = x.Result;
                    });
                }
                else
                {
                    //logging

                    //statusCode = System.Net.HttpStatusCode.ExpectationFailed;
                    //error = e.Message;
                }
            }

            return (result);
        }

        public async Task<string> GetDatabaseDefintions()
        {
            var result = await GetSingleItemRequest();
            return result;
        }


        //public async Task<Dictionary<string, double>> GetSelectedData(List<Field> Fields)
        public async Task<List<Parameters>> GetSelectedData(List<Field> Fields)
      
        {
            //List<string> keys = new List<string>(); IntegrationDataFilter integrationDataFilter = IntegrationDataFilter.s;
            //Dictionary<string, double> values = new Dictionary<string, double>();
            List<Parameters> values = new List<Parameters>();
            var result = await GetSingleItemRequest();
            JToken data;
            double value;
            bool converstionResult;

            for (int i = 0; i < Fields.Count(); i++)
            {
                data = JObject.Parse(result)[Fields[i].FielId];

                if (data.Type == JTokenType.Array)
                {
                    List<double> tmpList = new List<double>();
                    var tmp = data.Values<string>();
                    for (int j = 0; j < tmp.Count(); j++)
                    {
                        converstionResult = double.TryParse(data.Value<string>(), out value);
                        if (!converstionResult)
                        {
                            throw new Exception("Error in data");
                        }
                        tmpList.Add(value);
                    }

                    //if (integrationDataFilter == IntegrationDataFilter.First)
                    //{
                    //    values.Add(keys[i], tmpList.FirstOrDefault());
                    //}
                    //else if (integrationDataFilter == IntegrationDataFilter.Last)
                    //{
                    //    values.Add(keys[i], tmpList.LastOrDefault());
                    //}
                    if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Count.ToString().ToLower())
                    {
                        //values.Add(Fields[i].FielId, tmpList.Count());
                        values.Add(new Parameters
                        {

                            FieldId = Fields[i].FielId,
                            Value = Convert.ToString( tmpList.Count())

                        });
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Min.ToString().ToLower())
                    {
                        //values.Add(Fields[i].FielId, tmpList.Min());
                        values.Add(new Parameters
                        {

                            FieldId = Fields[i].FielId,
                            Value = Convert.ToString(tmpList.Min())

                        });
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Max.ToString().ToLower())
                    {
                        // values.Add(Fields[i].FielId, tmpList.Max());
                        values.Add(new Parameters
                        {

                            FieldId = Fields[i].FielId,
                            Value = Convert.ToString(tmpList.Max())

                        });
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Sum.ToString().ToLower())
                    {
                        // values.Add(Fields[i].FielId, tmpList.Sum());
                        values.Add(new Parameters
                        {

                            FieldId = Fields[i].FielId,
                            Value = Convert.ToString(tmpList.Sum())

                        });
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Average.ToString().ToLower())
                    {
                        //values.Add(Fields[i].FielId, tmpList.Average());
                        values.Add(new Parameters
                        {

                            FieldId = Fields[i].FielId,
                            Value = Convert.ToString(tmpList.Average())

                        });
                    }
                }
                else
                {
                    converstionResult = double.TryParse(data.Value<string>(), out value);
                    if (!converstionResult)
                        throw new Exception("Error in data");
                    // values.Add(Fields[i].FielId, Math.Round(value,2));
                    values.Add(new Parameters
                    {

                        FieldId = Fields[i].FielId,
                        Value = Convert.ToString(Math.Round(value, 2))

                    });
                }
            }
            return values;
        }
    }
}
