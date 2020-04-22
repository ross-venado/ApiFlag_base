using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    [MetadataType(typeof(RegisterUserModel.MetaData))]
    public partial class RegisterUserModel
    {

        sealed class MetaData
        {
            [Required]
            public string Nombre { get; set; }
            public string apellido { get; set; }
            [Required]
            public string Correo { get; set; }
            public string Direccion { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string Genero { get; set; }
            public string clave { get; set; }

            public string Token { get; set; }

            public string TipoLogin { get; set; }

        }
    }
}