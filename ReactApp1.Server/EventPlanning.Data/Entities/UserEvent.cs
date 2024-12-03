namespace EventPlanning.Data.Entities
{
    public class UserEvent
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public bool? EmailConfirmed { get; set; } = false;
    }
}
