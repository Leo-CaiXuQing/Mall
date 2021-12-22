using Mall.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Mall.Utility
{
    public class WebClientService : IWebClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public WebClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeOut">超时限制</param>
        /// <returns></returns>
        public async Task<WebClientResultDto> OnGetAsync(string url, int timeOut = 60, Dictionary<string, string> header = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (header != null)
                {
                    foreach (string item in header.Keys)
                        request.Headers.Add(item, header[item]);
                }
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(timeOut);

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return new WebClientResultDto()
                    {
                        status = response.StatusCode,
                        data = await response.Content.ReadAsStringAsync(),
                        message = "success"
                    };
                }

                return new WebClientResultDto()
                {
                    status = response.StatusCode,
                    message = "fail," + response.RequestMessage
                };
            }
            catch (Exception ex)
            {
                return new WebClientResultDto()
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "fail," + ex.Message
                };
            }
        }


        /// <summary>
        /// POST請求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="content">请求正文</param>
        /// <param name="timeOut">超时限制</param>
        /// <returns></returns>
        public async Task<WebClientResultDto> OnPostAsync(string url, string content = null, int timeOut = 60, Dictionary<string, string> header = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                if (!string.IsNullOrWhiteSpace(content))
                    request.Content = new StringContent(content);
                if (header != null)
                {
                    foreach (string item in header.Keys)
                    {
                        if (item.ToLower().Trim() == "content-type")
                        {
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(header[item]);
                            continue;
                        }
                        request.Headers.Add(item, header[item]);
                    }

                }
                else
                {
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(timeOut);

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return new WebClientResultDto()
                    {
                        status = response.StatusCode,
                        data = await response.Content.ReadAsStringAsync(),
                        message = "success"
                    };
                }

                return new WebClientResultDto()
                {
                    status = response.StatusCode,
                    message = "fail," + response.RequestMessage
                };
            }
            catch (Exception ex)
            {
                return new WebClientResultDto()
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "fail," + ex.Message
                };
            }
        }

    }

}
