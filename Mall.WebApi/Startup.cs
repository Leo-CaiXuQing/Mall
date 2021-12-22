using Autofac;
using Mall.WebApi.ApiGroup;
using Mall.WebApi.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mall.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //配置jwt认证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    //發佈者
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],

                    //驗證持有者
                    ValidateAudience = true,
                    ValidAudience = Configuration["Authentication:Audience"],

                    //token是否過期
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(secretByte)

                };
            }).AddCookie();

            services.AddAuthorization();

            //配置跨域处理
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.WithMethods("GET", "POST", "HEAD", "PUT", "PUTCH", "DELETE", "OPTIONS")
                    .AllowAnyOrigin();//允许任何来源的主机访问
                                      //.AllowCredentials()//指定处理cookie
                });

            });

            //注入HttpContext的访问器对象
            services.AddHttpContextAccessor();

            //掃描Profile文件,主要用于实体类和Dto进行映射
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //配置Swagger
            services.AddSwaggerGen(options =>
            {
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(item =>
                {
                    var info = item.GetCustomAttributes(typeof(GroupInfoAttribute), false)
                    .OfType<GroupInfoAttribute>().FirstOrDefault();

                    options.SwaggerDoc(item.Name, new OpenApiInfo()
                    {
                        Title = info?.Title,
                        Description = info?.Description,
                        Version = "v" + info?.Version,
                        Contact = new OpenApiContact() { Name = info?.Developer, Email = info?.Email }
                    });
                });

                options.SwaggerDoc("Default", new OpenApiInfo()
                {
                    Title = "Default"
                });

                options.DocInclusionPredicate((docName, apiDescription) =>
                {
                    return docName == "Default" ?
                    string.IsNullOrEmpty(apiDescription.GroupName) : apiDescription.GroupName == docName;
                });

                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Description = "JWT进行认证授权,直接在下框中输入Bearer {token}",
                    Name = "Authorization",//JWT默认参数名称
                    In = ParameterLocation.Header,//JWT默认存放Authorization信息的位置(请求头)
                    Type = SecuritySchemeType.ApiKey
                };

                options.AddSecurityDefinition("oauth2", openApiSecurityScheme);

                //添加请求头的Header中的token,传递到后台
                options.OperationFilter<AddResponseHeadersFilter>();

                //开启加权锁
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                options.OperationFilter<SecurityRequirementsOperationFilter>();

                //SwaggerUI设置xml文档注释路径
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Mall.WebApi.xml");
                options.IncludeXmlComments(xmlPath);

                //描述安全信息
                options.AddSecurityDefinition(CookieAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                {
                    Name = CookieAuthenticationDefaults.AuthenticationScheme,
                    Scheme = CookieAuthenticationDefaults.AuthenticationScheme
                });

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("any");

            app.UseStaticFiles(new StaticFileOptions() //静态文件配置
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(item =>
                {
                    var info = item.GetCustomAttributes(typeof(GroupInfoAttribute), false)
                                .OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerEndpoint(
                        $"/swagger/{item.Name}/swagger.json",
                        info != null ? info.Title : item.Name
                        );
                });
                options.SwaggerEndpoint("/swagger/Default/swagger.json", "Default");

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //注册Consul
            Configuration.ConsulRegister();

        }
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            //业务逻辑层所在程序集命名空间
            Assembly service = Assembly.Load("Mall.IService");

            //接口层所在程序集命名空间
            Assembly repository = Assembly.Load("Mall.Service");

            //自动注入
            containerBuilder.RegisterAssemblyTypes(repository, service)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();

            var controllerAssemblyType = typeof(Startup).Assembly.GetExportedTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToArray();
            containerBuilder.RegisterTypes(controllerAssemblyType).PropertiesAutowired();
        }

    }
}
