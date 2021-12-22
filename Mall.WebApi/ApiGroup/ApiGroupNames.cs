using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi.ApiGroup
{
    /// <summary>
    /// 系统分组枚举
    /// </summary>
    public enum ApiGroupNames
    {
        [GroupInfo(
            Title = "授权模块",
            Description = "授权模块相关接口文档",
            Version = 1,
            Developer = "Leo",
            Email = "leo.xq.cai@outlook.com")]
        Auth,

        [GroupInfo(
          Title = "用戶模块",
          Description = "用戶模块相关接口文档",
          Version = 1,
          Developer = "Leo",
          Email = "leo.xq.cai@outlook.com")]
        Users,
        [GroupInfo(
                 Title = "健康检查模块",
                 Description = "Consul Chek相关接口文档",
                 Version = 1,
                 Developer = "Leo",
                 Email = "leo.xq.cai@outlook.com")]
        Health,
        [GroupInfo(
                 Title = "开发者人员",
                 Description = "开发者人员文档",
                 Version = 1,
                 Developer = "Developer",
                 Email = "leo.xq.cai@outlook.com")]
        Developer
    }
}
