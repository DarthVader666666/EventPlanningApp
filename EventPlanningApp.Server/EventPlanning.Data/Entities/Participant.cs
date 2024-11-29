namespace EventPlanning.Data.Entities
{
    public class Participant
    {
        public int ParticipantId { get; set; }
        public int? ThemeId { get; set; }
        public string? ParticipantName { get; set; }
        public Theme? Theme { get; set; }
    }
}
