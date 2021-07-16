using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ApiApp.Controllers
{
    public class TestController : ApiController
    {
        // GET: Test
        /// <summary>
        /// Get Methode
        /// </summary>
        /// <returns>list of string</returns>
        public IEnumerable<string> Get()
        {
            var data = new string [] { "value1", "value2" };
            return (IEnumerable<string>)Json(data);
        }

        // GET api/values/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public HttpResponseMessage ValidateLogin(string userName, string userPassword)
        {
            if(userName == "khan" && userPassword =="khan")
            {
                return Request.CreateResponse(HttpStatusCode.OK, TokenManager.GenerateToken(userName)); 
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, "user name and password is not valid.");
            }
        }

        [HttpGet]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetEmployee()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Successfull");
        }
    }
}
