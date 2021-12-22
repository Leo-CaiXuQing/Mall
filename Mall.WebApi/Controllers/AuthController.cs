
using Mall.IService.User;
using Mall.Model.Dtos;
using Mall.WebApi.ApiGroup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mall.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Auth)]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        /// <param name="loginDto">用户名</param> 
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            //1.验证登录
            var user = _userService.FindUserByName(loginDto.UserName, loginDto.Password);
            if (user != null)
            {
                //2.創建JWT 
                //設置Token的加密算法
                var signingAlogorithm = SecurityAlgorithms.HmacSha256;

                //資源所有權,表述用戶身份,說明用戶角色,表示用戶所具有的權限
                var claims = new List<Claim>() { new Claim(JwtRegisteredClaimNames.Sub, user.UserName) };

                //獲取用戶角色
                var userRoles = user.Roles;

                foreach (var roleName in userRoles)
                {
                    var roleClaim = new Claim(ClaimTypes.Role, roleName);
                    claims.Add(roleClaim);
                }

                //header payload signiture
                //3.return 200 + JWT
                var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
                var signingKey = new SymmetricSecurityKey(secretByte);
                var signingCredentials = new SigningCredentials(signingKey, signingAlogorithm);

                var token = new JwtSecurityToken(
                     issuer: _configuration["Authentication:Issuer"],//發佈者
                     audience: _configuration["Authentication:Audience"],//授權者
                     claims,
                     notBefore: DateTime.UtcNow,//創建時間
                     expires: DateTime.UtcNow.AddDays(1), //一天內有效,
                     signingCredentials
                     );

                var tokenTostr = new JwtSecurityTokenHandler().WriteToken(token);
                return
                    Ok(new
                    {
                        success = true,
                        data = tokenTostr
                    });

            }

            return
                Ok(new
                {
                    success = false,
                    data = "失败!"
                });

        }

    }
}
