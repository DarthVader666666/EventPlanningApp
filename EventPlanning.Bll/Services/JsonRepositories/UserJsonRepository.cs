using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using JsonFlatFileDataStore;

namespace EventPlanning.Bll.Services.JsonRepositories
{
    public class UserJsonRepository : IRepository<User>
    {
        private readonly IDocumentCollection<User> _userCollection;
        private readonly IDocumentCollection<UserRole> _userRoleCollection;
        private readonly CryptoService _cryptoService;
        private int nextUserId;

        public UserJsonRepository(DataStore dataStore, CryptoService cryptoService)
        {
            _userCollection = dataStore.GetCollection<User>();
            _userRoleCollection = dataStore.GetCollection<UserRole>();
            _cryptoService = cryptoService;

            var collection = _userCollection.AsQueryable();
            nextUserId = !collection.Any() ? 1 : collection.Max(x => x.UserId) + 1;
        }

        public async Task<User?> CreateAsync(User item)
        {
            item.UserId = nextUserId++;
            await _userCollection.InsertOneAsync(item);
            await _userRoleCollection.InsertOneAsync(new UserRole { UserId = item.UserId, RoleId = 2 });

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

            string? email = null;
            int? id = null;
            var users = _userCollection.AsQueryable();

            if (IdOrEmail is string && users.Any())
            {
                email = (string?)IdOrEmail!;
                return Task.Run(() => users.FirstOrDefault(x => x.Email == email));
            }
            else if (IdOrEmail is int)
            {
                id = (int?)IdOrEmail;
                return Task.Run(() => users.FirstOrDefault(x => x.UserId == id));
            }

            return Task.FromResult<User?>(null);
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
