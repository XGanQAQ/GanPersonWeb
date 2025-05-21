namespace GanPersonWeb.Shared.DTO
{
    // 用户表
    public class User
    {
        public int Id { get; set; } // 主键
        public string Username { get; set; } = string.Empty; // 用户名
        public string Password { get; set; } = string.Empty; // 密码（加密存储）
        public string Role { get; set; } = "User"; // 角色（如管理员、普通用户）

        public static implicit operator User(Data.User model) =>
            new User
            {
                Id = model.Id,
                Username = model.Username,
                Password = model.Password,
                Role = model.Role
            };
    }

    // 个人信息表
    public class PersonalInfo
    {
        public int Id { get; set; } // 主键
        public string Name { get; set; } = string.Empty; // 姓名
        public string Occupation { get; set; } = string.Empty; // 职业
        public string Description { get; set; } = string.Empty; // 个人简介
        public string Email { get; set; } = string.Empty; // 电子邮箱
        public string ProfileImageUrl { get; set; } = string.Empty; // 个人头像URL

        // 导航属性
        public List<SocialMediaLink> SocialMediaLinks { get; set; } = new(); // 社交媒体链接

        public static implicit operator PersonalInfo(Data.PersonalInfo model) =>
            new PersonalInfo
            {
                Id = model.Id,
                Name = model.Name,
                Occupation = model.Occupation,
                Description = model.Description,
                Email = model.Email,
                ProfileImageUrl = model.ProfileImageUrl,
                SocialMediaLinks = model.SocialMediaLinks?.Select(link => (SocialMediaLink)link).ToList() ?? new()
            };
    }

    // 社交媒体链接表
    public class SocialMediaLink
    {
        public int Id { get; set; } // 主键
        public string Platform { get; set; } = string.Empty; // 平台名称（如GitHub、LinkedIn）
        public string IconUrl { get; set; } = string.Empty; // 图标URL
        public string Url { get; set; } = string.Empty; // 链接

        public static implicit operator SocialMediaLink(Data.SocialMediaLink model) =>
            new SocialMediaLink
            {
                Id = model.Id,
                Platform = model.Platform,
                IconUrl = model.IconUrl,
                Url = model.Url
            };
    }

    // 项目表
    public class Project
    {
        public int Id { get; set; } // 主键
        public string Title { get; set; } = string.Empty; // 项目标题
        public string Description { get; set; } = string.Empty; // 项目描述
        public string ImageUrl { get; set; } = string.Empty; // 项目预览图片URL
        public DateTime PublishDate { get; set; } // 项目发布日期
        public List<string> Tags { get; set; } = new(); // 项目标签
        public string Link { get; set; } = string.Empty; // 项目链接

        public static implicit operator Project(Data.Project model) =>
            new Project
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                PublishDate = model.PublishDate,
                Tags = model.Tags?.ToList() ?? new(),
                Link = model.Link
            };
    }

    // 博客表
    public class Blog
    {
        public int Id { get; set; } // 主键
        public string Title { get; set; } = string.Empty; // 博客标题
        public string Description { get; set; } = string.Empty; // 博客描述
        public string Content { get; set; } = string.Empty; // 博客内容
        public string HtmlContent { get; set; } = string.Empty; // 博客HTML内容
        public string ImageUrl { get; set; } = string.Empty; // 博客预览图片URL
        public DateTime PublishDate { get; set; } // 博客发布日期
        public string Type { get; set; } = string.Empty; // 博客类型（如技术、生活）
        public List<string> Tags { get; set; } = new(); // 博客标签
        public int ViewCount { get; set; } // 浏览量
        public int TalkCount { get; set; } // 评论量

        // 导航属性
        public List<Comment> Comments { get; set; } = new();

        public static implicit operator Blog(Data.Blog model) =>
            new Blog
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                HtmlContent = model.HtmlContent,
                ImageUrl = model.ImageUrl,
                PublishDate = model.PublishDate,
                Type = model.Type,
                Tags = model.Tags?.ToList() ?? new(),
                ViewCount = model.ViewCount,
                TalkCount = model.TalkCount,
                Comments = model.Comments?.Select(c => (Comment)c).ToList() ?? new()
            };
    }

    // 评论表
    public class Comment
    {
        public int Id { get; set; } // 主键
        public string Author { get; set; } = string.Empty; // 评论作者
        public string Content { get; set; } = string.Empty; // 评论内容
        public DateTime PublishDate { get; set; } // 评论发布日期
        public int? ReplyTo { get; set; } // 回复的评论ID（如果是回复）

        public static implicit operator Comment(Data.Comment model) =>
            new Comment
            {
                Id = model.Id,
                Author = model.Author,
                Content = model.Content,
                PublishDate = model.PublishDate,
                ReplyTo = model.ReplyTo
            };
    }

    // 图片表
    public class Image
    {
        public int Id { get; set; } // 主键
        public string Url { get; set; } = string.Empty; // 图片URL
        public DateTime UploadDate { get; set; } // 上传时间
        public string Description { get; set; } = string.Empty; // 图片描述
        public string Tags { get; set; } = string.Empty; // 图片标签

        public static implicit operator Image(Data.Image model) =>
            new Image
            {
                Id = model.Id,
                Url = model.Url,
                UploadDate = model.UploadDate,
                Description = model.Description,
                Tags = model.Tags
            };
    }

    public class SiteVisit
    {
        public int Id { get; set; }
        public DateTime VisitDate { get; set; } // 日期
        public int Count { get; set; } // 当天访问次数
        public string IpAddress { get; set; } = string.Empty; // IP地址
        public string area { get; set; } = string.Empty; // 访问区域

        public static implicit operator SiteVisit(Data.SiteVisit model) =>
            new SiteVisit
            {
                Id = model.Id,
                VisitDate = model.VisitDate,
                Count = model.Count,
                IpAddress = model.IpAddress,
                area = model.area
            };
    }
}
