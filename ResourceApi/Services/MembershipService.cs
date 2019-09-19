using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using ResourceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ResourceApi.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly HttpClient _clinet;
        public MembershipService(HttpClient client)
        {
            _clinet = client;
        }

        public async Task<string> GetAccessToken(LoginModel model)
        {
            _clinet.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var tokenResponse = await _clinet.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = "http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/token",
                ClientId = "btc",
                ClientSecret = "381c1d29-2309-4529-a268-df162b0ec74c",
                UserName = model.Email,
                Password = model.Pasword
            });
            return tokenResponse.AccessToken;
        }

        public async Task<UserModel> GetUserInfo(dynamic token)
        {
            _clinet.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token.Value);
            var userInfo = await _clinet.GetAsync("http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/userinfo");
            var user = await userInfo.Content.ReadAsAsync<UserModel>();
            return user;
        }

        public async Task<JsonResult> Logout(dynamic accessToken, string refreshToken)
        {
            _clinet.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            _clinet.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(accessToken.Value);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/logout");
            request.Content = new StringContent(refreshToken);
            var returnValue = await _clinet.SendAsync(request).ContinueWith(response =>
            {
                return new JsonResult(new { response.Result.StatusCode, response.Result.ReasonPhrase });
            });
            return returnValue;
        }
    }
}
