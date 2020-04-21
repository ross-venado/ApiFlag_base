using ApiFlag.DB;
using ApiFlag.Models;
using ApiFlag.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiFlag.Controllers
{


    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/image")]
    

    public class ImageController : ApiController
    {

        [HttpPost]
        [Route("UploadImage")]
        public async System.Threading.Tasks.Task<IHttpActionResult> UploadFileAsync()
        {


            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }


            ResponseModel response = await ImageService.UploadImageAsync(Request);
            return Ok(response);

        }


        [HttpGet]
        [Route("GetImage/{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response =  ImageService.GetImage(id);
            return response;

        }





    }
}
