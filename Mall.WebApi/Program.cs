using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //支持命令行
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .Build();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, loggerBuilder) =>
                {
                    //扩展日期
                    //忽略项
                    loggerBuilder.AddFilter("System", LogLevel.Warning);
                    loggerBuilder.AddFilter("Microsoft", LogLevel.Warning);
                    //替换为Log4Net
                    loggerBuilder.AddLog4Net();
                })
                //替换为Autofac容器
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
