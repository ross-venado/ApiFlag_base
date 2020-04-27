using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    public class usuarioModel
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public object Latitud { get; set; }
        public string Longitud { get; set; }
        public string Sexo { get; set; }
        public object jwt { get; set; }

        public string avatar { get; set; } 

    }
}