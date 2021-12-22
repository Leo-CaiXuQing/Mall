using Mall.WebApi.ApiGroup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Developer)]
    public class LeoController : ControllerBase
    {
        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            JObject json = new JObject();
            json.Add("昵称", "Leo");
            json.Add("用户名", "蔡序庆");
            json.Add("性别", "男");
            json.Add("邮箱", "leo.xq.cai@outlook.com");
            json.Add("手机号码", "17688923450");
            json.Add("签名", "彪悍的人生不需要解释,彪悍的代码不需要注释");
            return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(json));
        }

        [HttpGet]
        [Route("Sayhello")]
        public IActionResult SayHello()
        {
            return Ok("hello world!");
        }
    }
}
