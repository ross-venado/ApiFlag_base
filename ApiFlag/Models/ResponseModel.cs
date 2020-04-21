using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ApiFlag.Models
{
    public class ResponseModel
    {
        public string RsCode { get; set; }
        public string RsMessage { get; set; }
        public string RsMessageForUser { get; set; }
        public object RsContent { get; set; }

    }
}