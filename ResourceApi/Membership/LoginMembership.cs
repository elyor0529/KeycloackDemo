using IdentityModel.Client;
using ResourceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ResourceApi.Membership
{
    public class LoginMembership
    {
        public static async Task<string> GetAccessToken(HttpClient client, LoginModel model)
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
            return tokenResponse.AccessToken;
        }
    }
}
