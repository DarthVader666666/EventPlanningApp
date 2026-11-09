using EventPlanning.Api.Models;
using EventPlanning.Data.Entities;

using Mapster;

using MapsterMapper;

using System.Globalization;

namespace EventPlanning.Api.Configurations
{
    public static class MapsterConfiguration
    {
        public static void ConfigureAutomapper(this IServiceCollection services)
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<EventIndexModel, Event>();

            config.NewConfig<Event, EventIndexModel>()
                .Map(eim => eim.ThemeName, e => e.Theme != null ? e.Theme.ThemeName : null)
                .Map(eim => eim.SubThemeName, e => e.SubTheme != null ? e.SubTheme.SubThemeName : null)
                .Map(eim => eim.Date, e => e.Date != null
                    ? ((DateTime)e.Date).ToString("dddd, dd MMMM yyyy HH:mm", new CultureInfo("en-GB"))
                    : "");

            config.NewConfig<EventCreateModel, Event>()
                .Map(e => e.Participants, ecm => ecm.Participants != null
                    ? string.Join(",", ecm.Participants)
                    : null);

            config.NewConfig<Theme, ThemeIndexModel>()
                .Map(tm => tm.SubThemes, t => GetSubThemes(t.SubThemes));

            config.NewConfig<UserRegisterModel, User>();

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
        }

        private static SubThemeIndexModel[]? GetSubThemes(ICollection<SubTheme>? subThemes)
        {
            if (subThemes == null || subThemes.Count == 0)
            {
                return null;
            }

            IEnumerable<SubThemeIndexModel> GetSubThemesCore()
            {
                foreach (var subTheme in subThemes!)
                {
                    yield return new SubThemeIndexModel()
                    {
                        SubThemeId = subTheme?.SubThemeId,
                        SubThemeName = subTheme?.SubThemeName
                    };
                }
            }

            return GetSubThemesCore().ToArray();
        }
    }
}