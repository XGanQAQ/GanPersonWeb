using GanPersonWeb.Shared.Models;

namespace GanPersonWeb.Services
{
    public class PersonalInfoService
    {
        private readonly DatabaseService _databaseService;

        public PersonalInfoService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<PersonalInfo?> GetPersonalInfoAsync()
        {
            return await _databaseService.GetAllAsync<PersonalInfo>().ContinueWith(t => t.Result.FirstOrDefault());
        }

        public async Task UpdatePersonalInfoAsync(PersonalInfo personalInfo)
        {
            await _databaseService.UpdateAsync(personalInfo);
        }

        public async Task EnsureDefaultPersonalInfoAsync()
        {
            var existingInfo = await GetPersonalInfoAsync();
            if (existingInfo == null)
            {
                var defaultInfo = new PersonalInfo
                {
                    Name = "Ĭ������",
                    Occupation = "Ĭ��ְҵ",
                    Description = "����Ĭ�ϵĸ��˼�顣",
                    Email = "default@example.com",
                    ProfileImageUrl = "https://example.com/default-profile.png",
                    SocialMediaLinks = new List<SocialMediaLink>()
                };

                await _databaseService.AddAsync(defaultInfo);
            }
        }
    }
}
