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

            //����jwt��֤
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    //�l����
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],

                    //��C������
                    ValidateAudience = true,
                    ValidAudience = Configuration["Authentication:Audience"],

                    //token�Ƿ��^��
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(secretByte)

                };
            }).AddCookie();

            services.AddAuthorization();

            //���ÿ�����
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.WithMethods("GET", "POST", "HEAD", "PUT", "PUTCH", "DELETE", "OPTIONS")
                    .AllowAnyOrigin();//�����κ���Դ����������
                                      //.AllowCredentials()//ָ������cookie
                });

            });

            //ע��HttpContext�ķ���������
            services.AddHttpContextAccessor();

            //����Profile�ļ�,��Ҫ����ʵ�����Dto����ӳ��
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //����Swagger
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
                    Description = "JWT������֤��Ȩ,ֱ�����¿�������Bearer {token}",
                    Name = "Authorization",//JWTĬ�ϲ�������
                    In = ParameterLocation.Header,//JWTĬ�ϴ��Authorization��Ϣ��λ��(����ͷ)
                    Type = SecuritySchemeType.ApiKey
                };

                options.AddSecurityDefinition("oauth2", openApiSecurityScheme);

                //�������ͷ��Header�е�token,���ݵ���̨
                options.OperationFilter<AddResponseHeadersFilter>();

                //������Ȩ��
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                options.OperationFilter<SecurityRequirementsOperationFilter>();

                //SwaggerUI����xml�ĵ�ע��·��
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Mall.WebApi.xml");
                options.IncludeXmlComments(xmlPath);

                //������ȫ��Ϣ
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

            app.UseStaticFiles(new StaticFileOptions() //��̬�ļ�����
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

            //ע��Consul
            Configuration.ConsulRegister();

        }
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            //ҵ���߼������ڳ��������ռ�
            Assembly service = Assembly.Load("Mall.IService");

            //�ӿڲ����ڳ��������ռ�
            Assembly repository = Assembly.Load("Mall.Service");

            //�Զ�ע��
            containerBuilder.RegisterAssemblyTypes(repository, service)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();

            var controllerAssemblyType = typeof(Startup).Assembly.GetExportedTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToArray();
            containerBuilder.RegisterTypes(controllerAssemblyType).PropertiesAutowired();
        }

    }
}
