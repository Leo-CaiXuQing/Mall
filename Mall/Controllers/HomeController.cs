using Mall.IService.User;
using Mall.Model.Dtos;
using Mall.Models;
using Mall.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Consul;

namespace Mall.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IWebClientService _webClientHelper;
        //private static int iSeed = 0;
        public HomeController(ILogger<HomeController> logger, IUserService userService, IWebClientService webClientHelper)
        {
            _logger = logger;
            _userService = userService;
            _webClientHelper = webClientHelper;
        }

        /// <summary>
        /// 使用OceLot+Consul实现负载均衡
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var jsonContent = new JObject
            {
                { "userName", "Leo" },
                { "password", "123" }
            };
            //这个地址为API网关对外地址
            string url = "http://localhost:12321/service";
            WebClientResultDto webClientResultDto = await _webClientHelper.OnPostAsync(url + "/Auth/Login", jsonContent.ToString(), 60);
            if (webClientResultDto.status == HttpStatusCode.OK)
            {
                Dictionary<string, string> header = new Dictionary<string, string>()
                {
                    {"Authorization","Bearer "+ JObject.Parse(webClientResultDto.data)["data"].ToString()}
                };

                webClientResultDto = await _webClientHelper.OnGetAsync(url + "/user/GetUsers", 60, header);
                if (webClientResultDto.status == HttpStatusCode.OK)
                {
                    IEnumerable<UserDto> userDto = Newtonsoft.Json
           .JsonConvert.DeserializeObject<IEnumerable<UserDto>>(webClientResultDto.data);
                    ViewBag.UserAll = userDto;
                }
                return View();
            }
            else
            {
                return NotFound(webClientResultDto.message);
            }

        }

        /// <summary>
        /// 沒用使用API Gateway,通过Consul实现负载均衡实例
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index_()
        {
            var jsonContent = new JObject
            {
                { "userName", "Leo" },
                { "password", "123" }
            };

            string url = "http://mall";
            #region 使用Consul
            {
                Uri uri = new Uri(url);
                string groupName = uri.Host;
                ConsulClient consulClient = new ConsulClient(c =>
                {
                    c.Address = new Uri("http://localhost:8500/");
                    c.Datacenter = "leo-mall";
                });
                var response = consulClient.Agent.Services().Result.Response;

                //这里可以做负载均衡策略
                var services = response
                    .Where(v => v.Value
                    .Service.Equals(groupName, StringComparison.OrdinalIgnoreCase)).ToList();

                //负载均衡策略
                {
                    //取第一次,永远取第一个,由于Consul一分钟后才删除服务，所以如果第一个挂了一分钟再次请求会出现无法响应
                    //url = $@"{uri.Scheme}://{services.First().Value.Address}:{services.FirstOrDefault().Value.Port}";

                    //多个服务实例随机分配
                    url = $@"{uri.Scheme}://{services[new Random(new Guid().GetHashCode()).Next(0, services.Count)].Value.Address}:{services[new Random().Next(0, services.Count)].Value.Port}";

                    //轮询方式实现多个服务实例
                    //private static int iSeed=0;
                    // url = $@"{uri.Scheme}://{services[iSeed++ % services.Count].Value.Address}:{services[iSeed++ % services.Count].Value.Port}";

                    #region 根据权重进行分配
                    //List<KeyValuePair<string, AgentService>> keyValuePairs = new List<KeyValuePair<string, AgentService>>();
                    //foreach (var item in services)
                    //{
                    //    int count = int.Parse(item.Value.Tags?[0]);
                    //    for (int i = 0; i < count; i++)
                    //        keyValuePairs.Add(item);
                    //}
                    //AgentService agentService = keyValuePairs[new Random(iSeed++).Next(0, keyValuePairs.Count)].Value;
                    //url = $@"{uri.Scheme}://{agentService.Address}:{agentService.Port}"; 
                    #endregion
                }
                _logger.LogWarning($@"使用{url}调用的");
            }
            #endregion

            WebClientResultDto webClientResultDto = await _webClientHelper.OnPostAsync(url + "/api/Auth/Login", jsonContent.ToString(), 60);
            if (webClientResultDto.status == HttpStatusCode.OK)
            {
                Dictionary<string, string> header = new Dictionary<string, string>()
                {
                    {"Authorization","Bearer "+ JObject.Parse(webClientResultDto.data)["data"].ToString()}
                };

                webClientResultDto = await _webClientHelper.OnGetAsync(url + "/api/user/GetUsers", 60, header);
                if (webClientResultDto.status == HttpStatusCode.OK)
                {
                    IEnumerable<UserDto> userDto = Newtonsoft.Json
           .JsonConvert.DeserializeObject<IEnumerable<UserDto>>(webClientResultDto.data);
                    ViewBag.UserAll = userDto;
                }
            }
            else
            {
                ViewBag.UserAll = new List<UserDto>();
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
