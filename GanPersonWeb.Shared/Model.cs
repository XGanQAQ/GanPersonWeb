namespace GanPersonWeb.Shared.Models
{
    // 用户表
    public class User
    {
        public int Id { get; set; } // 主键
        public string Username { get; set; } = string.Empty; // 用户名
        public string Password { get; set; } = string.Empty; // 密码（加密存储）
        public string Role { get; set; } = "User"; // 角色（如管理员、普通用户）
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
        public List<SocialMediaLink> SocialMediaLinks { get; set; } = new(); // 社交媒体链接
    }

    // 社交媒体链接表
    public class SocialMediaLink
    {
        public int Id { get; set; } // 主键
        public string Platform { get; set; } = string.Empty; // 平台名称（如GitHub、LinkedIn）
        public string Url { get; set; } = string.Empty; // 链接
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
    }

    // 博客表
    public class Blog
    {
        public int Id { get; set; } // 主键
        public string Title { get; set; } = string.Empty; // 博客标题
        public string Description { get; set; } = string.Empty; // 博客描述
        public string Content { get; set; } = string.Empty; // 博客内容
        public string ImageUrl { get; set; } = string.Empty; // 博客预览图片URL
        public DateTime PublishDate { get; set; } // 博客发布日期
        public string Type { get; set; } = string.Empty; // 博客类型（如技术、生活）
        public List<string> Tags { get; set; } = new(); // 博客标签
        public int ViewCount { get; set; } // 浏览量
        public int TalkCount { get; set; } // 评论量
    }

    // 评论表
    public class Comment
    {
        public int Id { get; set; } // 主键
        public int BlogId { get; set; } // 关联的博客ID
        public string Author { get; set; } = string.Empty; // 评论作者
        public string Content { get; set; } = string.Empty; // 评论内容
        public DateTime PublishDate { get; set; } // 评论发布日期
        public int? ReplyTo { get; set; } // 回复的评论ID（如果是回复）
    }

    // 图片表
    public class Image
    {
        public int Id { get; set; } // 主键
        public string Url { get; set; } = string.Empty; // 图片URL
        public DateTime UploadDate { get; set; } // 上传时间
        public string Description { get; set; } = string.Empty; // 图片描述
        public string Tags { get; set; } = string.Empty; // 图片标签
    }

    public class SiteVisit
    {
        public int Id { get; set; }
        public DateTime VisitDate { get; set; } // 日期
        public int Count { get; set; } // 当天访问次数
        public string IpAddress { get; set; } = string.Empty; // IP地址
        public string area { get; set; } = string.Empty; // 访问区域
    }
}
