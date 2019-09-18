using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ResourceApi.Membership;
using ResourceApi.Models;

namespace ResourceApi.Controllers
{
    [Route("/keycloak/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(model);

            using (var client = _clientFactory.CreateClient())
            {
                var result = await LoginMembership.GetAccessToken(client, model);
                return Ok(result);
            }
        }
        

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Info()
        {
            var tokenValue = HttpContext.Request.Headers.SingleOrDefault(s => s.Key == "Authorization");
            using (var client = _clientFactory.CreateClient())
            {
                var user = await UserInfoMembership.GetUserInfo(client, tokenValue);
                return Ok(user);
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<HttpResponseMessage> Logout()
        {
            var result = new HttpResponseMessage();
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                var jsonBody = JObject.Parse(body);
                var refreshToken = jsonBody.GetValue("refresh_token").ToString();
                using (var client = _clientFactory.CreateClient())
                {
                    var tokenValue = HttpContext.Request.Headers.SingleOrDefault(s => s.Key == "Authorization");
                    result = await LogoutMembership.Logout(client, tokenValue, refreshToken);
                }
            }
            return result;
        }

    }
}
