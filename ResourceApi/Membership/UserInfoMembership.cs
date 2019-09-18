using ResourceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ResourceApi.Membership
{
    public class UserInfoMembership
    {
        public static async Task<UserModel> GetUserInfo(HttpClient client, dynamic token)
        {
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token.Value);
            var userInfo = await client.GetAsync("http://178.33.123.109:8080/auth/realms/dev/protocol/openid-connect/userinfo");
            var user = await userInfo.Content.ReadAsAsync<UserModel>();
            return user;
        }
    }
}
