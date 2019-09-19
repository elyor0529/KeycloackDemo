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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResourceApi.Models;
using ResourceApi.Services;

namespace ResourceApi.Controllers
{
    [Route("/keycloak/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public AccountController(IHttpClientFactory clientFactory, IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpPost]
        public async Task<dynamic> Login([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(model);

            var result = await _membershipService.GetAccessToken(model);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Info()
        {
            var tokenValue = HttpContext.Request.Headers.SingleOrDefault(s => s.Key == "Authorization");
            var user = await _membershipService.GetUserInfo(tokenValue);
            //var user = await UserInfoMembership.GetUserInfo(client, tokenValue);
            return Ok(user);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<JsonResult> Logout()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                var jsonBody = JObject.Parse(body);
                var credsModel = new ClientCredsModel();
                credsModel.refresh_token = jsonBody.GetValue("refresh_token").ToString();
                credsModel.client_id = jsonBody.GetValue("client_id").ToString();
                var convertedModel = JsonConvert.SerializeObject(credsModel);

                var tokenValue = HttpContext.Request.Headers.SingleOrDefault(s => s.Key == "Authorization");
                var returnValue = await _membershipService.Logout(tokenValue, convertedModel);
                return returnValue;
            }
        }
    }
}
