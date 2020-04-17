using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    [MetadataType(typeof(LoginRequest.MetaData))]
    public partial class LoginRequest
    {


        sealed class MetaData
        {
            [Required]
            public string Username { get; set; }



            [Required]
            public string Password { get; set; }

            [Required]
            public string Recurso { get; set; }

            [Required]
            public string Appautentication { get; set; }
        }



    }
}