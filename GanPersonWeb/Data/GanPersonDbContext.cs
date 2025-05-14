using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GanPersonWeb.Data
{
    public class GanPersonDbContext : DbContext
    {
        public GanPersonDbContext(DbContextOptions<GanPersonDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<PersonalInfo> PersonalInfos { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; } // 新增评论表
        public DbSet<SiteVisit> SiteVisits { get; set; }
    }
}
