using AutoMapper;
using EventPlanning.Api.Models;
using EventPlanning.Data.Entities;
using System.Globalization;

namespace EventPlanning.Api.Configurations
{
    public static class AutomapperConfiguration
    {
        public static void ConfigureAutomapper(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var config = new MapperConfiguration(autoMapperConfig =>
                {
                    autoMapperConfig.CreateMap<EventIndexModel, Event>();

                    autoMapperConfig.CreateMap<Event, EventIndexModel>()
                        .ForMember(eim => eim.ThemeName, opt => opt.MapFrom(e => e.Theme != null ? e.Theme.ThemeName : null))
                        .ForMember(eim => eim.SubThemeName, opt => opt.MapFrom(e => e.SubTheme != null ? e.SubTheme.SubThemeName : null))
                        .ForMember(eim => eim.Date, opt => opt.MapFrom(e => e.Date != null ? ((DateTime)e.Date).ToString("dddd, dd MMMM yyyy HH:mm", new CultureInfo("en-GB")) : ""));

                    autoMapperConfig.CreateMap<EventCreateModel, Event>()
                        .ForMember(e => e.Participants, opt => opt.MapFrom(ecm => ecm.Participants != null ? string.Join(",", ecm.Participants) : null));

                    autoMapperConfig.CreateMap<Theme, ThemeIndexModel>()
                        .ForMember(tm => tm.SubThemes, opt => opt.MapFrom(t => GetSubThemes(t.SubThemes)));

                    autoMapperConfig.CreateMap<UserRegisterModel, User>();
                });

                return config.CreateMapper();
            });
        }

        //private static string[]? GetSubThemeNames(ICollection<SubTheme>? subThemes)
        //{ 
        //    if(subThemes == null || subThemes.Count == 0)
        //    {
        //        return null;
        //    }

        //    IEnumerable<string?> GetNames()
        //    {
        //        foreach (var subTheme in subThemes!)
        //        { 
        //            yield return subTheme?.SubThemeName;
        //        }
        //    }

        //    return GetNames().ToArray();
        //}

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