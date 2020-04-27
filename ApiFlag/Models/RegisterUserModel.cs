using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    public partial class RegisterUserModel
    {

        public string Nombre { get; set; }
        public string apellido { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Genero { get; set; }
        public string clave { get; set; }

        public string Token { get; set; }

        public string TipoLogin { get; set; }

        public string avatar { get; set; }


    }
}