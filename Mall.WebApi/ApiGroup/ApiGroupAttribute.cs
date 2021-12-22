using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi.ApiGroup
{
    public class ApiGroupAttribute : Attribute, IApiDescriptionGroupNameProvider
    {
        /// <summary>
        /// 系统分组特性
        /// </summary>
        public ApiGroupAttribute(ApiGroupNames groupName)
        {
            GroupName = groupName.ToString();
        }
        public string GroupName { get; set; }

    }
}
