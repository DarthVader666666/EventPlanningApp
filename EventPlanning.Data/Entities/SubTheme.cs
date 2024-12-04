namespace EventPlanning.Data.Entities
{
    public class SubTheme
    {
        public int SubThemeId { get; set; }
        public int? ThemeId { get; set; }
        public string? SubThemeName { get; set; }
        public Theme? Theme { get; set; }
    }
}
