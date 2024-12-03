using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using JsonFlatFileDataStore;
using Microsoft.IdentityModel.Tokens;

namespace EventPlanning.Bll.Services.JsonRepositories
{
    public class UserJsonRepository : IRepository<User>
    {
        private readonly IDocumentCollection<User> _userCollection;
        private int nextUserId;

        public UserJsonRepository(DataStore dataStore)
        {
            _userCollection = dataStore.GetCollection<User>();
            var collection = _userCollection.AsQueryable();
            nextUserId = collection.IsNullOrEmpty() ? 1 : collection.Max(x => x.UserId) + 1;
        }

        public async Task<User?> CreateAsync(User item)
        {
            item.UserId = nextUserId++;
            await _userCollection.InsertOneAsync(item);
            return item;
        }

        public Task<User?> DeleteAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string? email)
        {
            if (email == null)
            {
                return Task.FromResult<User?>(null);
            }

            var users = _userCollection.AsQueryable();

            return Task.Run(() => users.Any() ? users.FirstOrDefault(x => x.Email == (string?)email) : null);
        }

        public Task<User?> GetAsync(object? IdOrEmail)
        {
            if (IdOrEmail == null)
            {
                return Task.FromResult<User?>(null);
            }

            var users = _userCollection.AsQueryable();

            return Task.Run(() => users.Any() 
                ? users.FirstOrDefault(x => IdOrEmail is string && ((string?)IdOrEmail!).Contains('@') ? x.Email == (string?)IdOrEmail : x.UserId == (int?)IdOrEmail) 
                : null);
        }

        public Task<IEnumerable<User?>> GetListAsync(object? id = null)
        {
            return Task.Run(() => _userCollection.AsQueryable());
        }

        public Task<User?> UpdateAsync(User item)
        {
            throw new NotImplementedException();
        }
    }
}
