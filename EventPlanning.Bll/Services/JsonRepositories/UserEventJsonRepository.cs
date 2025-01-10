using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using JsonFlatFileDataStore;

namespace EventPlanning.Bll.Services.JsonRepositories
{
    public class UserEventJsonRepository : IRepository<UserEvent>
    {
        private readonly IDocumentCollection<UserEvent> _userEventCollection;

        public UserEventJsonRepository(DataStore dataStore)
        {
            _userEventCollection = dataStore.GetCollection<UserEvent>();
        }

        public async Task<UserEvent?> CreateAsync(UserEvent item)
        {
            var isCreated = await _userEventCollection.InsertOneAsync(item);
            return isCreated ? item : null;
        }

        public Task<UserEvent?> DeleteAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(UserEvent? item)
        {
            return await GetAsync(new Tuple<int?, int?>(item?.UserId, item?.EventId)) != null;
        }

        public Task<UserEvent?> GetAsync(object? value)
        {
            var tuple = (Tuple<int?, int?>?)value;

            var userEvent = _userEventCollection.AsQueryable().FirstOrDefault(x => x.UserId == tuple?.Item1 && x.EventId == tuple.Item2);

            return Task.Run(() => userEvent);
        }

        public Task<IEnumerable<UserEvent?>> GetListAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserEvent?> UpdateAsync(UserEvent item)
        {
            var isUpdated = await _userEventCollection.UpdateOneAsync(x => x.EventId == item.EventId, item);
            return isUpdated ? item : null;
        }
    }
}
