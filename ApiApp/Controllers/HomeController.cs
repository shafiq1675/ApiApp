using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ApiApp.Controllers
{
    public class HomeController : Controller
    {
        private static string WebApiURL = "http://localhost:59114/api/test";
        public async Task<ActionResult> Index()
        {
            var tokenbase = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebApiURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseMessage = await client.GetAsync("Test/ValidateLogin?userName=khan&userPassword=khan");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    tokenbase = JsonConvert.DeserializeObject<string>(result);
                    Session["TokenNumber"] = tokenbase;
                    Session["UserName"] = "khan";
                }
            }

            return Content(tokenbase);
        }

        public async Task<ActionResult> GetEmployee()
        {
            var resultMessage = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebApiURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["TokenNumber"].ToString() + ":" + Session["UserName"].ToString()); //
                var responseMessage = await client.GetAsync("Test/GetEmployee");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    resultMessage = JsonConvert.DeserializeObject<string>(result);
                }
            }

            return Content(resultMessage);
        }
    }
}
