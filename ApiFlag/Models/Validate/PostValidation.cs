using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    [MetadataType(typeof(PostModel.MetaData))]
    public partial  class PostModel
    {

        sealed class MetaData
        {
            public int post_id { get; set; }

            [Required]
            public string post_mensaje { get; set; }

            public int id_flag { get; set; }

            public int id_usuario { get; set; }

            public string post_latitud { get; set; }

            public string post_longitud { get; set; }

            public int post_estado { get; set; }

            public DateTime post_ts { get; set; }
        }
    }
}