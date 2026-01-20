using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class ResultWrapper<T>
    {
        public ResultWrapper()
        {
            this.StatusCode = "Success";
        }
        public ResultWrapper(string StatusCode, string StatusMessage, string ErrorCode, T Data)
        {
            this.StatusCode = StatusCode;
            this.StatusMessage = StatusMessage;
            this.ErrorCode = ErrorCode;
            this.Data = Data;
        }
        public string StatusCode { set; get; }
        public string StatusMessage { set; get; }
        public string ErrorCode { set; get; }
        public T Data { set; get; }
    }
}
