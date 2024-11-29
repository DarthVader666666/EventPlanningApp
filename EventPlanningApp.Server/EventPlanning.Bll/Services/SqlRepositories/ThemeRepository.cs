using EventPlanning.Bll.Interfaces;
using EventPlanning.Data;
using EventPlanning.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Bll.Services.SqlRepositories
{
    public class ThemeRepository : IRepository<Theme>
    {
        private readonly EventPlanningDbContext _dbContext;

        public ThemeRepository(EventPlanningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Theme?> CreateAsync(Theme item)
        {
            throw new NotImplementedException();
        }

        public Task<Theme?> DeleteAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Theme item)
        {
            throw new NotImplementedException();
        }

        public Task<Theme?> GetAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Theme?>> GetListAsync(object? id)
        {
            var themes = _dbContext.Themes.Include(x => x.SubThemes);
            return await Task.Run(() => themes.ToList());
        }

        public Task<Theme?> UpdateAsync(Theme item)
        {
            throw new NotImplementedException();
        }
    }
}
