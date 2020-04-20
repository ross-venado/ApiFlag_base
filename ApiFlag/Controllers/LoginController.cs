using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using ApiFlag.Models;
using ApiFlag.Services;
using System.Web.Http.Cors;

namespace ApiFlag.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "http://localhost:8100", headers: "*", methods: "*")]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        // Modelo de respuestas personalizadas
        

        [HttpPost]
        [Route("verify")]
        public IHttpActionResult verify(LoginRequest login)
        {
            ResponseModel response = UserServices.VerifyModel(login.Username, login.Password);
            return Ok(response);
            //return Ok(customerFake);
        }


    }
}


