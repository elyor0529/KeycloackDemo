using Microsoft.AspNetCore.Mvc;
using ResourceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceApi.Services
{
    public interface IMembershipService
    {
        Task<string> GetAccessToken(LoginModel model);
        Task<UserModel> GetUserInfo(dynamic token);
        Task<JsonResult> Logout(dynamic accessToken, string refreshToken);   
    }
}
