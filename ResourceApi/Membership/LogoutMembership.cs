using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ResourceApi.Membership
{
    public class LogoutMembership
    {
        public static async Task<HttpResponseMessage> Logout(HttpClient client, dynamic accessToken, string refreshToken)
        {
            var result = new HttpResponseMessage();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(accessToken.Value);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/logout");
            request.Content = new StringContent(refreshToken);
            await client.SendAsync(request).ContinueWith(response =>
            {
                result = response.Result;
            });
            return result;
        }
    }
}
