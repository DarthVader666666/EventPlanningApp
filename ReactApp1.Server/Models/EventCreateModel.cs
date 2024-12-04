namespace EventPlanning.Api.Models
{
    public class EventCreateModel
    {
        public string? Title { get; set; }
        public string? DressCode { get; set; }
        public int? ThemeId { get; set; }
        public int? SubThemeId { get; set; }
        public string[]? Participants { get; set; }
        public string? Location { get; set; }
        public int? AmountOfVacantPlaces { get; set; }
        public DateTime? Date { get; set; }
    }
}
