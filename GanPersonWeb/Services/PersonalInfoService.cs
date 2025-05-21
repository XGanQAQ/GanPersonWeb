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
            var personInfoList = await _databaseService.GetAllAsync<PersonalInfo>();
            var personInfo = personInfoList.FirstOrDefault();
            if (personInfo == null)
            {
                return null;
            }
            personInfo.SocialMediaLinks = await _databaseService.GetAllAsync<SocialMediaLink>();
            return personInfo;
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
                    ProfileImageUrl = "/images/head.jpg",
                    SocialMediaLinks = new List<SocialMediaLink>()
                    {
                        new SocialMediaLink
                        {
                            Platform = "GitHub",
                            IconUrl = "/icons/github.svg",
                            Url = "https://github.com/"
                        },
                        new SocialMediaLink
                        {
                            Platform = "Twitter",
                            IconUrl = "/icons/twitter.svg",
                            Url = "https://twitter.com/"
                        },
                    }
                };

                await _databaseService.AddAsync(defaultInfo);
            }
        }
    }
}
