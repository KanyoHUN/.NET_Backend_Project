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

        protected bool ValidateUser(User userToValidate)
        {
            //Validation Logic will be here
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
        
        public async Task<int> CreateUser(User userToBeCreated) //To be modified with validation
        {
            _context.Users.Add(userToBeCreated);
            await SaveCahngesAsyncService();
            return 0;
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
