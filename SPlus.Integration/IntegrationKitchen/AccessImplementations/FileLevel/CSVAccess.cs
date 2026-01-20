using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using SPlus.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static SPlus.Integration.Enums;

namespace SPlus.Integration.IntegrationKitchen.AccessImplementations.FileLevel
{
    public class CSVAccess : IAccess
    {
        string filePath = "test.csv";
        Dictionary<string, double> data;

        public async Task<string> GetDatabaseDefintions()
        {
            return ReadFile();
        }

        //public async Task<Dictionary<string, double>> GetSelectedData(List<Field> Fields)
        //{
        //List<string> keys = new List<string>(); IntegrationDataFilter integrationDataFilter = IntegrationDataFilter.Sum;
        //ReadData(Fields, integrationDataFilter);
        //return data;
        //}

        public async Task<List<Parameters>> GetSelectedData(List<Field> Fields)
        {
            return new List<Parameters>();
        }

        private string ReadFile()
        {
            List<string> headers = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    headers.Add(values[1]);
                    break;
                }
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(headers);
            return jsonString.ToString();
        }

        private string ReadData(List<Field> Fields, Enums.IntegrationDataFilter integrationDataFilter)
        {
            data = new Dictionary<string, double>();
            double outValue;
            List<double> dataValues = new List<double>();

            string[] headers;
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    headers = line.Split(',');
                    break;
                }
            }

            using (var reader = new StreamReader(filePath))
            {
                int i = 0;
                while (!reader.EndOfStream)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (double.TryParse("", out outValue))
                    {
                        dataValues.Add(outValue);
                    }
                    else
                    {
                        throw new Exception("Data fomrat is not valid!");
                    }

                    break;
                }
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject("");
            return jsonString.ToString();
        }
    }
}
