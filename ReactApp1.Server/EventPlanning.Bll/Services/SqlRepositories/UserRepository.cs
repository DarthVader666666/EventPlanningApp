﻿using EventPlanning.Bll.Interfaces;
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
            _dbContext.Users.Add(item);
            await _dbContext.SaveChangesAsync();
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

            var user = _dbContext.Users.Any()
                ? _dbContext.Users.FirstOrDefault(x =>
                IdOrEmail is string && ((string?)IdOrEmail!).Contains('@') ? x.Email == (string?)IdOrEmail : x.UserId == (int?)IdOrEmail)
                : null;

            return Task.Run(() => user);
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
