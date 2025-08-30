using Microsoft.AspNetCore.Mvc;

using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello from my WebAPI!";
        }

        [HttpPost]
        public string Post([FromBody] HelloRequest request)
        {
            try
            {
                Console.WriteLine(request.Data);
                return request?.Data ?? "No data received";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "No data received";
            }
        }
    }
}
