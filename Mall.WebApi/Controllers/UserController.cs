using AutoMapper;
using Mall.IService.User;
using Mall.Model.Dtos;
using Mall.Model.Entity;
using Mall.WebApi.ApiGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mall.WebApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Users)]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UserController(
            ILogger<UserController> logger,
            IMapper mapper,
            IUserService userService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 按Id查询用户信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}", Name = "GetUserById")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "管理员")]
        public IActionResult GetUserById(int Id)
        {
            try
            {
                _logger.LogWarning("我从哪里来的" + _configuration["port"]);
                //两种方式都可以
                //var dto = _mapper.Map<UserInfo, UserDto>(_userService.FindUser(Id));
                var dto = _mapper.Map<UserDto>(_userService.FindUser(Id));
                return Ok(dto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "管理员")]
        public IActionResult GetUsers()
        {
            try
            {
                var dto = _mapper.Map<IEnumerable<UserDto>>(_userService.UserAll());
                return Ok(dto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 查询用户资料
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserInfo")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetUserInfo()
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext.User.
                            FindFirst(ClaimTypes.NameIdentifier).Value;
                var dto = _mapper.Map<UserInfo, UserDto>(_userService.FindUserByName(userName));
                return Ok(dto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

    }
}
