using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_manager_backend.Models;

namespace user_manager_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserDbContext _context;

        public UserController(UserDbContext context)
        {
            _context = context;
        }
    }
}
