using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Mall.Model.Dtos
{
    public class WebClientResultDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode status { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string message { get; set; }
    }
}
