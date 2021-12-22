using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi.ApiGroup
{
    /// <summary>
    /// 系统接口相关说明
    /// </summary>
    public class GroupInfoAttribute : Attribute
    {
        public string Title { get; set; }
        public int Version { get; set; }
        public string Description { get; set; }
        public string Developer { get; set; }
        public string Email { get; set; }
    }
}
