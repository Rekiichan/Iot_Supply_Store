using IotSupplyStore.DataAccess;
using IotSupplyStore.Models;
using IotSupplyStore.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;

namespace IotSupplyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        [HttpGet("{id}")]
        public async Task<IEnumerable<ApplicationUser>> GetEmail(string id)
        {

            var ExistsUser = await _db.User.FirstOrDefaultAsync(u => u.Id == id);
            if (ExistsUser == null)
            {
                return (IEnumerable<ApplicationUser>)NotFound($"User is null");
            }

            var rng = new Random();
            var message = new Message(new string[] { ExistsUser.Email }, "Register Success!", "Change password");
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
