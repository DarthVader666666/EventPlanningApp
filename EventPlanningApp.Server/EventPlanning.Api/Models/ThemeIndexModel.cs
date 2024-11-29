namespace EventPlanning.Api.Models
{
    public class ThemeIndexModel
    {
        public int? ThemeId { get; set; }
        public string? ThemeName { get; set; }
        public ICollection<SubThemeIndexModel>? SubThemes { get; set; }
    }
}
