using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                //var response = client.PostAsync("http://178.33.123.109:8080/auth/realms/dev", new FormUrlEncodedContent(null));
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = "http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/token",
                    ClientId = "btc",
                    ClientSecret = "381c1d29-2309-4529-a268-df162b0ec74c",
                    UserName = model.Email,
                    Password = model.Pasword
                });
                // return tokenResponse.AccessToken;
                //token op kelas usr/psw yozib keyloack apidan!
                // return Ok("{token}");
                return Ok(tokenResponse.AccessToken);
            }
        }

        //Authorize-> hamas tekshrad,siza kkli schema k,basic template kkmas bizga detalni xatozi yozin
        //Gitignore oqshin ahjma foile trash,2-3 line bn ish bitmed

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Info()
        {
            //bizaga kkmas bu,qolgan reallni info kk 4ta columns.
            //Keycloak api bor bor remote ulanas ,Keycloak API ga,ozimiz api ortada ortakas bolad.
            using (var client = _clientFactory.CreateClient())
            {
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = "http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/token",
                    ClientId = "btc",
                    ClientSecret = "381c1d29-2309-4529-a268-df162b0ec74c",
                    UserName = "abror",
                    Password = "abror"
                });
                client.SetBearerToken(tokenResponse.AccessToken);
                var userInfo = await client.GetAsync("http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/userinfo");
                var user = await userInfo.Content.ReadAsAsync<UserModel>();
                return Ok(user);
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            //remote session qop ketsa,har doim prev sessiondag token ishlatmes,shuni un session yopib yurish kk

            return Ok();
        }

    }
}
