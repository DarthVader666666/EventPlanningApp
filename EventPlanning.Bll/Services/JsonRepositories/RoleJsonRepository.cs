﻿using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using JsonFlatFileDataStore;

namespace EventPlanning.Bll.Services.JsonRepositories
{
    public class RoleJsonRepository : IRepository<Role>
    {
        private readonly IDocumentCollection<Role> _roleCollection;
        private readonly IDocumentCollection<UserRole> _userRoleCollection;
        private int nextRoleId;

        public RoleJsonRepository(DataStore dataStore)
        {
            _roleCollection = dataStore.GetCollection<Role>();
            _userRoleCollection = dataStore.GetCollection<UserRole>();
            var collection = _roleCollection.AsQueryable();
            nextRoleId = !collection.Any() ? 1 : collection.Max(x => x.RoleId) + 1;
        }

        public async Task<Role?> CreateAsync(Role item)
        {
            item.RoleId = nextRoleId++;
            var result = await _roleCollection.InsertOneAsync(item);

            return result ? item : null;
        }

        public Task<Role?> DeleteAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Role? item)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> GetAsync(object? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role?>> GetListAsync(object? userId)
        {
            return Task.FromResult<IEnumerable<Role?>>(_userRoleCollection.AsQueryable().Where(x => x.UserId == (int?)userId)
                .SelectMany(userRole => _roleCollection.AsQueryable().Where(role => userRole.RoleId == role.RoleId)));
        }

        public Task<Role?> UpdateAsync(Role item)
        {
            throw new NotImplementedException();
        }
    }
}
