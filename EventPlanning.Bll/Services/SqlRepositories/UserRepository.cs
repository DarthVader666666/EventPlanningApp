using EventPlanning.Bll.Interfaces;
using EventPlanning.Data;
using EventPlanning.Data.Entities;

namespace EventPlanning.Bll.Services.SqlRepositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly EventPlanningDbContext _dbContext;

        public UserRepository(EventPlanningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> CreateAsync(User item)
        {
            var user = _dbContext.Users.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            _dbContext.UserRoles.Add(new UserRole { UserId = user.UserId });
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public Task<User?> DeleteAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(User? item)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string? email)
        {
            if (email == null)
            {
                return Task.FromResult<User?>(null);
            }

            var user = _dbContext.Users.Any()
                ? _dbContext.Users.FirstOrDefault(x => x.Email == (string?)email)
                : null;

            return Task.Run(() => user);
        }

        public Task<User?> GetAsync(object? IdOrEmail)
        {
            if (IdOrEmail == null)
            {
                return Task.FromResult<User?>(null);
            }

            string? email = null;
            int? id = null;

            if (IdOrEmail is string && _dbContext.Users.Any())
            {
                email = (string?)IdOrEmail!;
                return Task.Run(() => _dbContext.Users.FirstOrDefault(x => x.Email == email));
            }
            else if (IdOrEmail is int)
            {
                id = (int?)IdOrEmail;
                return Task.Run(() => _dbContext.Users.FirstOrDefault(x => x.UserId == id));
            }

            return Task.FromResult<User?>(null);
        }

        public Task<IEnumerable<User?>> GetListAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> UpdateAsync(User item)
        {
            throw new NotImplementedException();
        }
    }
}
