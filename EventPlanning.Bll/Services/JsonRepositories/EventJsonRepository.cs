using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using JsonFlatFileDataStore;

namespace EventPlanning.Bll.Services.JsonRepositories
{
    public class EventJsonRepository : IRepository<Event>
    {
        private readonly IDocumentCollection<Event> _eventCollection;
        private readonly IDocumentCollection<Theme> _themeCollection;
        private readonly IDocumentCollection<SubTheme> _subThemeCollection;
        private int nextEventId;

        public EventJsonRepository(DataStore dataStore)
        {
            _eventCollection = dataStore.GetCollection<Event>();
            _themeCollection = dataStore.GetCollection<Theme>();
            _subThemeCollection = dataStore.GetCollection<SubTheme>();
            var collection = _eventCollection.AsQueryable();
            nextEventId = !collection.Any() ? 1 : collection.Max(x => x.EventId) + 1;
        }

        public async Task<Event?> CreateAsync(Event item)
        {
            item.EventId = nextEventId++;
            var result = await _eventCollection.InsertOneAsync(item);
            return result ? item : null;
        }

        public async Task<Event?> DeleteAsync(object? id)
        {
            var item = await GetAsync(id);

            if (!await ExistsAsync(item))
            { 
                return null;
            }

            var result = await _eventCollection.DeleteOneAsync(x => x.EventId == (int?)id);
            return result ? item : null;
        }

        public Task<bool> ExistsAsync(Event? item)
        {
            if (item == null)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(GetAsync(item.EventId) != null);
        }

        public Task<Event?> GetAsync(object? id)
        {
            var eventItem = _eventCollection.AsQueryable().FirstOrDefault(x => x.EventId == (int?)id);
            var theme = _themeCollection.AsQueryable().FirstOrDefault(x => x.ThemeId == eventItem?.ThemeId);
            var subTheme = _subThemeCollection.AsQueryable().FirstOrDefault(x => x.ThemeId == theme?.ThemeId);

            if (eventItem != null)
            {
                eventItem.Theme = theme;
                eventItem.SubTheme = subTheme;
            }            

            return Task.FromResult(eventItem);
        }

        public Task<IEnumerable<Event?>> GetListAsync(object? id)
        {
            var events = _eventCollection.AsQueryable().Select(e =>
            {
                var theme = _themeCollection.AsQueryable().FirstOrDefault(t => t.ThemeId == e.ThemeId);
                var subTheme = _subThemeCollection.AsQueryable().FirstOrDefault(s => e.SubThemeId == s.SubThemeId);
                e.Theme = theme;
                e.SubTheme = subTheme;

                return e;
            });

            return Task.FromResult<IEnumerable<Event?>>(events);
        }

        public async Task<Event?> UpdateAsync(Event item)
        {
            if (item == null)
            {
                return null;
            }

            await _eventCollection.UpdateOneAsync(e => e.EventId == item.EventId, item);

            return item;
        }
    }
}
