
using Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi.Utility
{
    public static class ConsulService
    {
        /// <summary>
        /// 服务注册中心
        /// </summary>
        /// <param name="configuration"></param>
        public static void ConsulRegister(this IConfiguration configuration)
        {
            ConsulClient consulClient = new ConsulClient(c =>
            {
                c.Address = new Uri("http://localhost:8500/");
                c.Datacenter = "leo-mall";
            });

            string ip = configuration["ip"];
            int port = int.Parse(configuration["port"]);
            int weight = string.IsNullOrWhiteSpace(configuration["weight"]) ? 1 : int.Parse(configuration["weight"]);

            consulClient.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "service-" + Guid.NewGuid(),//唯一标识
                Name = "Mall",//组名称Group
                Address = ip,//对应的IP
                Port = port, //端口
                Tags = new string[] { weight.ToString() },//標籤
                //Consul健康检查,定时调用检查服务是否可用
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(12), //定时检查,12秒一次
                    HTTP = $@"http://{ip}:{port}/api/Health/Index",//要检查的i地址
                    Timeout = TimeSpan.FromSeconds(5),//检查超时时间,5秒响应时间
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)//失败后多久移除此Service,最少1分钟
                }
            });
            //命令行参数获取
            Console.WriteLine(@$"ip={ip},port={port},weight={weight},service register success!");
        }
    }
}
