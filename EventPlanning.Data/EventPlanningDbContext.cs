using EventPlanning.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EventPlanning.Data
{
    public class EventPlanningDbContext:DbContext
    {
        private readonly IConfiguration _configuration;

        public EventPlanningDbContext(DbContextOptions<EventPlanningDbContext> options, IConfiguration configuration) : base(options) 
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user => 
            {
                user.HasKey(x => x.UserId);
                user.HasIndex(x => x.Email).IsUnique();
                user.HasData(new User
                {
                    UserId = 1,
                    FirstName = "Vadzim",
                    LastName = "Rumiantsau",
                    Email = _configuration["AdminEmail"],
                    Password = _configuration["AdminPassword"]
                });
            });
            modelBuilder.Entity<ParticipantEvent>().HasKey(x => new { x.EventId, x.ParticipantId });
            modelBuilder.Entity<UserEvent>().HasKey(x => new { x.EventId, x.UserId });
            modelBuilder.Entity<Role>(role =>
            {
                role.HasKey(x => x.RoleId);
                role.HasData(
                    new Role
                    {
                        RoleId = 1,
                        RoleName = "Admin",
                    },
                    new Role
                    {
                        RoleId = 2,
                        RoleName = "User",
                    });
            });
            modelBuilder.Entity<UserRole>(userRole => 
            { 
                userRole.HasKey(x => new { x.UserId, x.RoleId });
                userRole.HasData(new UserRole
                {
                    UserId = 1,
                    RoleId = 1
                });
            });
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<SubTheme> SubThemes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<ParticipantEvent> ParticipantEvents { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
