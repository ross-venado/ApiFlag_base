using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    public partial class FlagModel
    {

        public int id { get; set; }

        public string Descripcion { get; set; }

        public int estado { get; set; }
    }
}