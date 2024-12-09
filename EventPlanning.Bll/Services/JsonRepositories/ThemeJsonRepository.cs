using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using JsonFlatFileDataStore;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Bll.Services.JsonRepositories
{
    public class ThemeJsonRepository : IRepository<Theme>
    {
        private readonly IDocumentCollection<Theme> _themeCollection;
        private readonly IDocumentCollection<SubTheme> _subThemeCollection;
        private int nextThemeId;

        public ThemeJsonRepository(DataStore dataStore)
        {
            _themeCollection = dataStore.GetCollection<Theme>();
            _subThemeCollection = dataStore.GetCollection<SubTheme>();
            var collection = _themeCollection.AsQueryable();
            nextThemeId = !collection.Any() ? 1 : collection.Max(x => x.ThemeId) + 1;
        }

        public async Task<Theme?> CreateAsync(Theme item)
        {
            item.ThemeId = nextThemeId++;
            var result = await _themeCollection.InsertOneAsync(item);

            return result ? item : null;
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
            var themes = ((IQueryable<Theme>)_themeCollection.AsQueryable()).Include(x => x.SubThemes);
            return await Task.Run(() => themes.ToList());
        }

        public Task<Theme?> UpdateAsync(Theme item)
        {
            throw new NotImplementedException();
        }
    }
}
