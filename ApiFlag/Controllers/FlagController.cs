using ApiFlag.Models;
using ApiFlag.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;



namespace ApiFlag.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/flag")]
    public class FlagController : ApiController
    {

        [HttpPost]
        [Route("Create")]
        [ResponseType(typeof(FlagModel))]
        public IHttpActionResult Create(FlagModel flag)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                  

            ResponseModel response = FlagService.CreateFlag(flag);
            return Ok(response);
            //return Ok(customerFake);
        }



        [HttpPost]
        [Route("Update/{id:int}")]
        public IHttpActionResult Update(int id,FlagModel flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResponseModel response = FlagService.UpdateFlag(id,flag);
            return Ok(response);
        }



        [HttpPost]        
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            ResponseModel response = FlagService.DeleteFlag(id);
            return Ok(response);
        }


        // GEt: api/flag/flags
        [HttpGet]
        [Route("Flags")]
        public IHttpActionResult Flags()
        {
            ResponseModel response = FlagService.SelectFlags();
            return Ok(response);
        }


        [HttpGet]
        [Route("Flags/{id:int}")]
        public IHttpActionResult Flags(int id)
        {
            ResponseModel response = FlagService.SelectFlags(id);
            return Ok(response);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
