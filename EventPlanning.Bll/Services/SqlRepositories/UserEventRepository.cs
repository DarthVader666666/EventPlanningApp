using EventPlanning.Bll.Interfaces;
//using EventPlanning.Data;
//using EventPlanning.Data.Entities;

namespace EventPlanning.Bll.Services.SqlRepositories
{
    public class UserEventRepository// : IRepository<UserEvent>
    {
        //private readonly EventPlanningDbContext _dbContext;

        //public UserEventRepository(EventPlanningDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        //public async Task<UserEvent?> CreateAsync(UserEvent item)
        //{
        //    var created = _dbContext.UserEvents.Add(item);
        //    await _dbContext.SaveChangesAsync();
        //    return created.Entity;
        //}

        //public Task<UserEvent?> DeleteAsync(object? id)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<bool> ExistsAsync(UserEvent item)
        //{
        //    return await GetAsync(new Tuple<int?, int?>(item.UserId, item.EventId)) != null;
        //}

        //public Task<UserEvent?> GetAsync(object? value)
        //{
        //    var tuple = (Tuple<int?, int?>)value;

        //    var userEvent = _dbContext.UserEvents.FirstOrDefault(x => x.UserId == tuple.Item1 && x.EventId == tuple.Item2);

        //    return Task.Run(() => userEvent);
        //}

        //public Task<IEnumerable<UserEvent?>> GetListAsync(object? id)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<UserEvent?> UpdateAsync(UserEvent item)
        //{
        //    var result = _dbContext.UserEvents.Update(item);
        //    await _dbContext.SaveChangesAsync();
        //    return result.Entity;
        //}
    }
}
