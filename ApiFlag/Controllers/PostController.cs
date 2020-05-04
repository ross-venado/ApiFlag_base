using ApiFlag.Models;
using ApiFlag.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiFlag.Controllers
{
    [Authorize]
    [RoutePrefix("Api/Post")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PostController : ApiController
    {
        [HttpPost]
        [Route("New")]
        public IHttpActionResult Create(PostModel post)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            ResponseModel response = PostService.CreatePost(post);
            return Ok(response);
            //return Ok(customerFake);
        }





        [HttpGet]
        [Route("Posts")]
        public IHttpActionResult Posts([FromUri]PagingParameterModel pagin)
        {
            ResponseModel response = PostService.Posts(pagin);
            return Ok(response);
        }



        [HttpGet]
        [Route("Posts/{id:int}")]
        public IHttpActionResult Posts(int id, [FromUri]PagingParameterModel pagin)
        {
            ResponseModel response = PostService.Posts(pagin,id);
            return Ok(response);
        }



        [HttpGet]
        [Route("ValidateToken")]
        public IHttpActionResult ValidateToken()
        {
            ResponseModel response = PostService.ValidaToken();
            return Ok(response);
        }


    }
}
