using IotSupplyStore.DataAccess;
using IotSupplyStore.Models;
using IotSupplyStore.Models.UpsertModel;
using IotSupplyStore.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IotSupplyStore.Controllers
{
    [Route("api/customer")]
    //[Authorize] // Uncomment if has the identity
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            return await _db.Customers.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var obj = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (obj == null)
            {
                return NotFound($"Customer with id {id} is null");
            }

            CustomerVM result = new CustomerVM()
            {
                Id = id,
                FullName = obj.FullName,
                Phone = obj.Phone,
                Email = obj.Email,
                Avatar = obj.Avatar,
                Address = obj.Address,
                CreatedAt = obj.CreatedAt,
                UpdatedAt = obj.UpdatedAt,
                TransactionList = obj.TransactionList,
                Orders = obj.Orders,
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerUpsert newCustomer)
        {
            var checkExistCustomer = await _db.Customers.FirstOrDefaultAsync(u => u.FullName == newCustomer.FullName);
            if (checkExistCustomer != null)
            {
                return BadRequest("This Customer has already exist");
            }
            Customer customer = new Customer()
            {
                FullName = newCustomer.FullName,
                Phone = newCustomer.Phone,
                Email = newCustomer.Email,
                Avatar = newCustomer.Avatar,
                Address = newCustomer.Address,
                Password = newCustomer.Password
            };
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            //return Ok("Customer added");
            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerUpsert customerUpsert)
        {
            var ExistCustomer = await _db.Customers.FirstOrDefaultAsync(u => u.Id == id);
            if (ExistCustomer == null)
            {
                return NotFound($"Customer with id {id} is null");
            }

            ExistCustomer.FullName = customerUpsert.FullName;
            ExistCustomer.Phone = customerUpsert.Phone;
            ExistCustomer.Email = customerUpsert.Email;
            ExistCustomer.Avatar = customerUpsert.Avatar;
            ExistCustomer.Address = customerUpsert.Address;
            ExistCustomer.Password = customerUpsert.Password;
            ExistCustomer.UpdatedAt = DateTime.Now;

            _db.Entry(ExistCustomer).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok("Customer updated");
        }

        // DELETE: /users/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var user = await _db.Customers.FindAsync(id);

            if (user == null)
            {
                return NotFound($"Customer with id {id} is null");
            }

            _db.Customers.Remove(user);
            await _db.SaveChangesAsync();
            return Ok("Customer deleted");
        }
    }
}
