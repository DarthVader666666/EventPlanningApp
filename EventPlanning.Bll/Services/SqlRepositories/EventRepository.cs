using EventPlanning.Bll.Interfaces;
using EventPlanning.Data;
using EventPlanning.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Bll.Services.SqlRepositories
{
    public class EventRepository : IRepository<Event>
    {
        private readonly EventPlanningDbContext _dbContext;

        public EventRepository(EventPlanningDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Event?> CreateAsync(Event item)
        {
            await _dbContext.Events.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<Event?> DeleteAsync(object? id)
        {
            var eventItem = _dbContext.Events.FirstOrDefault(e => e.EventId == (int)(id ?? 0));

            if (eventItem == null)
            {
                return null;
            }

            _dbContext.Events.Remove(eventItem);
            await _dbContext.SaveChangesAsync();

            return eventItem;
        }

        public Task<bool> ExistsAsync(Event? item)
        {
            throw new NotImplementedException();
        }

        public async Task<Event?> GetAsync(object? id)
        {
            return await _dbContext.Events.Include(x => x.Theme).Include(x => x.SubTheme).FirstOrDefaultAsync(x => x.EventId == (int?)id);
        }

        public async Task<IEnumerable<Event?>> GetListAsync(object? id)
        {
            return await _dbContext.Events.Include(x => x.Theme).Include(x => x.SubTheme).ToListAsync();
        }

        public async Task<Event?> UpdateAsync(Event item)
        {
            if (item == null)
            {
                return null;
            }

            _dbContext.Events.Update(item);
            await _dbContext.SaveChangesAsync();

            return item;
        }
    }
}
