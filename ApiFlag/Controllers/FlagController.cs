using ApiFlag.Models;
using ApiFlag.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;



namespace ApiFlag.Controllers
{
    [Authorize]
    [AllowAnonymous]
    [RoutePrefix("api/flag")]
    public class FlagController : ApiController
    {

        [HttpPost]
        [Route("Create")]
        [ResponseType(typeof(FlagModel))]
        public IHttpActionResult Create(FlagModel flag)
        {
            ResponseModel response = FlagService.CreateFlag(flag);
            return Ok(response);
            //return Ok(customerFake);
        }



        [HttpPost]
        [Route("Update")]
        public IHttpActionResult Update(FlagModel flag)
        {
            ResponseModel response = FlagService.UpdateFlag(flag);
            return Ok(response);
        }



        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult Delete(FlagModel flag)
        {
            ResponseModel response = FlagService.DeleteFlag(flag);
            return Ok(response);
        }


    }
}
