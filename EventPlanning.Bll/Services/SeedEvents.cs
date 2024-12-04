using EventPlanning.Data;
using EventPlanning.Data.Entities;

namespace EventPlanning.Bll.Services
{
    public static class SeedEvents
    {
        public static void Seed(this EventPlanningDbContext dbContext)
        {
            dbContext.Events.AddRange(                
                new Event
                {
                    Title = "Test Title 1",
                    Theme = new Theme
                    {
                        ThemeName = "Test Theme 1",
                    },
                    Date = DateTime.Now.AddDays(0)
                },
                new Event
                {
                    Title = "Test Title 2",
                    Theme = new Theme
                    {
                        ThemeName = "Test Theme 2",
                    },
                    Date = DateTime.Now.AddDays(10)
                },
                new Event
                {
                    Title = "Test Title 3",
                    Theme = new Theme
                    {
                        ThemeName = "Test Theme 3",
                    },
                    Date = DateTime.Now.AddDays(20)
                }
            );

            dbContext.SaveChanges();
        }
    }
}
