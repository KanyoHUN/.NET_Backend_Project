using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace user_manager_backend.Models
{
    public class UserService
    {
        private ModelStateDictionary _modelstate;
        private UserDbContext _context;

        public UserService(ModelStateDictionary modelstate, UserDbContext context)
        {
            _modelstate = modelstate;
            _context = context;
        }

        public bool ValidateUser(User userToValidate)
        {
            HashSet<string> sexes = new HashSet<string>();
            sexes.Add("Male");
            sexes.Add("Female");

            if (userToValidate.Age < 1 | userToValidate.Age > 110)
            {
                _modelstate.AddModelError("Age", "Age cannot be outside of [1;110]");
            }

            if (userToValidate.Sex != null)
            {
                if (!sexes.Contains(userToValidate.Sex))
                {
                    _modelstate.AddModelError("Sex", "Sex has only two options! (Male,Female)");
                }
            }
            else
            {
                _modelstate.AddModelError("Sex", "Sex is required!");
            }

            if(userToValidate.Name == null | userToValidate.Name == "")
            {
                _modelstate.AddModelError("Name", "Name is required!");
            }

            return _modelstate.IsValid;
        }

        public bool CheckDbContext()
        {
            if (_context.Users == null)
            {
                return true;
            }

            return false;
        }

        public async Task<ActionResult<IEnumerable<User>>> ListUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<User>>> ListFilteredUsers(string name)
        {
            return await _context.Users.Where(user => user.Name == name).ToListAsync();
        }

        public async Task<User> ReturnUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            return user;
        }
        
        public async Task<bool> CreateUser(User userToBeCreated)
        {
            if (!ValidateUser(userToBeCreated))
            {
                return false;
            }

            try
            {
                _context.Users.Add(userToBeCreated);
                await SaveCahngesAsyncService();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<int> RemoveUser(User userToBeRemoved)
        {
            _context.Users.Remove(userToBeRemoved);
            await SaveCahngesAsyncService();
            return 0;
        }

        public void ModifyUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<int> SaveCahngesAsyncService()
        {
            await _context.SaveChangesAsync();
            return 0;
        }

        public bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
