using Microsoft.AspNetCore.Mvc;

namespace JenkinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JenkinCICDController : ControllerBase
    {
     
        [HttpPost]
        public IActionResult Add(int nums)
        {
            string ans = nums.ToString();
            return StatusCode(200,ans);   
        }

        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode(200, "Get working properly::::::::::::");
        }
    }
}
