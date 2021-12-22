using Castle.Core.Logging;
using Mall.WebApi.ApiGroup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi.Controllers
{
    [ApiGroup(ApiGroupNames.Health)]
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IConfiguration _configuration;

        public HealthController(ILogger<HealthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
          //  Console.WriteLine($@"this is ip={_configuration["ip"]} port= {_configuration["port"]} execute.");
            return NoContent();
        }
    }

}
