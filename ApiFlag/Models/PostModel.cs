using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    public partial class PostModel
    {


        public int post_id { get; set; }

        public string post_mensaje { get; set; }

        public int id_flag { get; set; }

        public int id_usuario { get; set; }

        public string nombre_usuario { get; set; }

        public string post_latitud { get; set; }

        public string post_longitud { get; set; }

        public int post_estado { get; set; }

        public DateTime post_ts { get; set; }

        public List<ImageModel> image { get; set; }


        

    }
}