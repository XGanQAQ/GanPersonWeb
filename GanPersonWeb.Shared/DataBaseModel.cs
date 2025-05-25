namespace GanPersonWeb.Shared.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

        public static implicit operator User(DTO.User dto) =>
            new User
            {
                Id = dto.Id,
                Username = dto.Username,
                Password = dto.Password,
                Role = dto.Role
            };
    }

    public class PersonalInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public List<SocialMediaLink> SocialMediaLinks { get; set; } = new();

        public static implicit operator PersonalInfo(DTO.PersonalInfo dto) =>
            new PersonalInfo
            {
                Id = dto.Id,
                Name = dto.Name,
                Occupation = dto.Occupation,
                Description = dto.Description,
                Email = dto.Email,
                ProfileImageUrl = dto.ProfileImageUrl,
                SocialMediaLinks = dto.SocialMediaLinks?.Select(link => (SocialMediaLink)link).ToList() ?? new()
            };
    }

    public class SocialMediaLink
    {
        public int Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int PersonalInfoId { get; set; }
        public PersonalInfo? PersonalInfo { get; set; }

        public static implicit operator SocialMediaLink(DTO.SocialMediaLink dto) =>
            new SocialMediaLink
            {
                Id = dto.Id,
                Platform = dto.Platform,
                IconUrl = dto.IconUrl,
                Url = dto.Url
                // PersonalInfoId 和 PersonalInfo 需在业务层补充
            };
    }

    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Link { get; set; } = string.Empty;

        public static implicit operator Project(DTO.Project dto) =>
            new Project
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                PublishDate = dto.PublishDate,
                Tags = dto.Tags?.ToList() ?? new(),
                Link = dto.Link
            };
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public int ViewCount { get; set; }
        public int TalkCount { get; set; }
        public List<Comment> Comments { get; set; } = new();

        public static implicit operator Blog(DTO.Blog dto) =>
            new Blog
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Content = dto.Content,
                HtmlContent = dto.HtmlContent,
                ImageUrl = dto.ImageUrl,
                PublishDate = dto.PublishDate,
                Type = dto.Type,
                Tags = dto.Tags?.ToList() ?? new(),
                ViewCount = dto.ViewCount,
                TalkCount = dto.TalkCount,
                Comments = dto.Comments?.Select(c => (Comment)c).ToList() ?? new()
            };
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public int? ReplyTo { get; set; }
        public int BlogId { get; set; }
        public Blog? Blog { get; set; }

        public static implicit operator Comment(DTO.Comment dto) =>
            new Comment
            {
                Id = dto.Id,
                Author = dto.Author,
                Content = dto.Content,
                PublishDate = dto.PublishDate,
                ReplyTo = dto.ReplyTo
                // BlogId 和 Blog 需在业务层补充
            };
    }

    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;

        public static implicit operator Image(DTO.Image dto) =>
            new Image
            {
                Id = dto.Id,
                Url = dto.Url,
                UploadDate = dto.UploadDate,
                Description = dto.Description,
                Tags = dto.Tags
            };
    }

    public class SiteVisit
    {
        public int Id { get; set; }
        public DateTime VisitDate { get; set; }
        public int Count { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string area { get; set; } = string.Empty;

        public static implicit operator SiteVisit(DTO.SiteVisit dto) =>
            new SiteVisit
            {
                Id = dto.Id,
                VisitDate = dto.VisitDate,
                Count = dto.Count,
                IpAddress = dto.IpAddress,
                area = dto.area
            };
    }
}
