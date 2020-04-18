using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiFlag.Models
{
    [MetadataType(typeof(FlagModel.MetaData))]
    public partial class FlagModel
    {
        sealed class MetaData
        {
            public int id { get; set; }

            [Required]
            public string Descripcion { get; set; }

            
            public int estado { get; set; }

        }


    }
}