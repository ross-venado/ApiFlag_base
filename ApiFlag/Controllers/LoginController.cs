﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using ApiFlag.Models;
using ApiFlag.Services;

namespace ApiFlag.Controllers
{
    [AllowAnonymous]
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



/*
 * 
 * ESTOS SON LOS OTROS METODO QUE NO UTILIZO HAHAHA XD
 * [HttpGet]
[Route("echoping")]
public IHttpActionResult EchoPing()
{
    return Ok(true);
}

[HttpGet]
[Route("echouser")]
public IHttpActionResult EchoUser()
{
    var identity = Thread.CurrentPrincipal.Identity;
    return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
}

            [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login)
        {
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            // Validamos el modelo.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //TODO: Validate credentials Correctly, this code is only for demo !!
            bool isCredentialValid = (login.Password == "123456");
            if (isCredentialValid)
            {
                var token = TokenGenerator.GenerateTokenJwt(login.Username);
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
        }
 
     
     
     */
