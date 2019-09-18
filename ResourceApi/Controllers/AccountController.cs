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
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = "http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/token",
                    ClientId = "btc",
                    ClientSecret = "381c1d29-2309-4529-a268-df162b0ec74c",
                    UserName = model.Email,
                    Password = model.Pasword
                });
                return Ok(tokenResponse.AccessToken);
            }
        }

        //Authorize-> hamas tekshrad,siza kkli schema k,basic template kkmas bizga detalni xatozi yozin
        //Gitignore oqshin ahjma foile trash,2-3 line bn ish bitmed

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Info()
        {
            var tokenValue = HttpContext.Request.Headers.SingleOrDefault(s => s.Key == "Authorization");
            using (var client = _clientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(tokenValue.Value);
                var userInfo = await client.GetAsync("http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/userinfo");
                var user = await userInfo.Content.ReadAsAsync<UserModel>();
                return Ok(user);
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public HttpResponseMessage Logout()
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

                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(tokenValue.Value);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/logout");
                    request.Content = new StringContent(refreshToken);
                    client.SendAsync(request).ContinueWith(response =>
                    {
                        result = response.Result;
                    });
                }
            }
            return result;
        }

    }
}
