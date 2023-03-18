using IotSupplyStore.Models;
using IotSupplyStore.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IotSupplyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        [HttpGet]
        public IEnumerable<ApplicationUser> Get()
        {
            var rng = new Random();
            var message = new Message(new string[] { "phamdinhtruong2511@gmail.com" }, "Test email", "This is the content from our email.");
            _emailSender.SendEmail(message);
            return Enumerable.Range(1, 5).Select(index => new ApplicationUser
            {
                FullName = index.ToString(),
                Address = index.ToString(),
                Email = index.ToString(),
            })
            .ToArray();
        }
    }
}
