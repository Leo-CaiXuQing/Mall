using Mall.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.Utility
{
    public interface IWebClientService
    {
        Task<WebClientResultDto> OnGetAsync(string url, int timeOut = 60, Dictionary<string, string> header = null);
        Task<WebClientResultDto> OnPostAsync(string url, string content, int timeOut, Dictionary<string, string> header = null);

    }
}
