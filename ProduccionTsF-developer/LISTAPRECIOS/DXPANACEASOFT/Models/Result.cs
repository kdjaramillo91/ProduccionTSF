using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DXPANACEASOFT.Models
{
    public class ApiResult
    {
        public int Code { get; set; }
        public int Value { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}