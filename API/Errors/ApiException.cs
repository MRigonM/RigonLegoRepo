using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiException : ApiResponse
    {
        public ApiException(int StatusCode, string Message = null,string Details = null) : base(StatusCode, Message)
        {
            this.Details = Details;
        }

        public String Details { get; set; }
    }
}