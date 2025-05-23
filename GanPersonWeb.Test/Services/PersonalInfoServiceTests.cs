using GanPersonWeb.Data;
using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GanPersonWeb.Test.Services
{
    public class PersonalInfoServiceTests
    {
        private DbContextOptions<GanPersonDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task GetPersonalInfo_ShouldReturnPersonalInfo()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var service = new PersonalInfoService(databaseService);

                var personalInfo = new PersonalInfo { Name = "Test Name", Occupation = "Test Occupation" };
                context.PersonalInfos.Add(personalInfo);
                await context.SaveChangesAsync();

                var result = await service.GetPersonalInfoAsync();
                Assert.NotNull(result);
                Assert.Equal("Test Name", result?.Name);
            }
        }

        [Fact]
        public async Task UpdatePersonalInfo_ShouldUpdatePersonalInfo()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var service = new PersonalInfoService(databaseService);

                var personalInfo = new PersonalInfo { Id = 1, Name = "Old Name", Occupation = "Old Occupation" };
                context.PersonalInfos.Add(personalInfo);
                await context.SaveChangesAsync();

                personalInfo.Name = "Updated Name";
                personalInfo.Occupation = "Updated Occupation";
                await service.UpdatePersonalInfoAsync(personalInfo);

                var updatedInfo = await context.PersonalInfos.FindAsync(1);
                Assert.NotNull(updatedInfo);
                Assert.Equal("Updated Name", updatedInfo?.Name);
            }
        }
    }
}
