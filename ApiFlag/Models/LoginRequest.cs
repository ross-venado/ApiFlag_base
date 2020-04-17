using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    public partial class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Recurso { get; set; }
        public string Appautentication { get; set; }
    }
}