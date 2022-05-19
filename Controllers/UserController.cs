using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_manager_backend.Models;

namespace user_manager_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _service;

        public UserController(UserDbContext context)
        {
            _service = new UserService(this.ModelState, context);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_service.CheckDbContext())
            {
                return NotFound();
            }

            var user = await _service.ReturnUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] string? name)
        {
            if (_service.CheckDbContext())
            {
                return NotFound();
            }

            if (name != null)
            {
                return await _service.ListFilteredUsers(name);
            }

            return await _service.ListUsers();
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (_service.CheckDbContext())
            {
                return NotFound();
            }

            if (await _service.CreateUser(user))
            {
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            if (!_service.ValidateUser(user))
            {
                return BadRequest();
            }

            _service.ModifyUser(user);

            try
            {
                await _service.SaveCahngesAsyncService();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_service.UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_service.CheckDbContext())
            {
                return NotFound();
            }

            var user = await _service.ReturnUser(id);

            if (user == null)
            {
                return NotFound();
            }

            await _service.RemoveUser(user);

            return NoContent();
        }
    }
}
