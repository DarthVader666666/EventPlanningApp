namespace EventPlanning.Data.Entities
{
    public class Event
    {
        public int EventId { get; set; }
        public string? Title { get; set; }
        public bool? DressCode { get; set; } = false;
        public DateTime? Date { get; set; }
        public int? AmountOfVacantPlaces { get; set; }
        public string? Address { get; set; }
        public int? ThemeId { get; set; }
        public int? SubThemeId { get; set; }
        public string? Location { get; set; }
        public string? Participants { get; set; }
        public Theme? Theme { get; set; }
        public SubTheme? SubTheme { get; set; }
    }
}
