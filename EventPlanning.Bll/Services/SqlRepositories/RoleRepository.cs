using EventPlanning.Bll.Interfaces;
using EventPlanning.Data;
using EventPlanning.Data.Entities;

namespace EventPlanning.Bll.Services.SqlRepositories
{
    public class RoleRepository : IRepository<Role>
    {
        private readonly EventPlanningDbContext _dbContext;

        public RoleRepository(EventPlanningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Role?> CreateAsync(Role item)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> DeleteAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Role item)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> GetAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Role>?> GetListAsync(object? userId)
        {
            return _dbContext.UserRoles.Where(x => x.UserId == (int?)userId)
                .SelectMany(userRole => _dbContext.Roles.Where(role => userRole.RoleId == role.RoleId));
        }

        public Task<Role?> UpdateAsync(Role item)
        {
            throw new NotImplementedException();
        }
    }
}
