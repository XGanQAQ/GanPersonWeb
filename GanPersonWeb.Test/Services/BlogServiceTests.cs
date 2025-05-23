using GanPersonWeb.Data;
using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GanPersonWeb.Test.Services
{
    public class BlogServiceTests
    {
        private DbContextOptions<GanPersonDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddBlog_ShouldAddBlog()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var blogService = new BlogService(databaseService);

                var blog = new Blog
                {
                    Title = "Test Blog",
                    Content = "This is a test blog.",
                    ImageUrl = "https://example.com/image.jpg",
                    PublishDate = DateTime.Now,
                    Tags = new List<string> { "Test", "Blog" },
                    ViewCount = 0,
                    TalkCount = 0
                };

                await blogService.AddBlogAsync(blog);

                Assert.Single(await context.Blogs.ToListAsync());
            }
        }

        [Fact]
        public async Task GetBlogs_ShouldReturnAllBlogs()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var blogService = new BlogService(databaseService);

                var blog1 = new Blog { Title = "Blog 1", Content = "Content 1" };
                var blog2 = new Blog { Title = "Blog 2", Content = "Content 2" };

                context.Blogs.Add(blog1);
                context.Blogs.Add(blog2);
                await context.SaveChangesAsync();

                var blogs = await blogService.GetBlogsAsync();
                Assert.Equal(2, blogs.Count);
            }
        }

        [Fact]
        public async Task GetBlogById_ShouldReturnCorrectBlog()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var blogService = new BlogService(databaseService);

                var blog = new Blog { Title = "Blog 1", Content = "Content 1" };
                context.Blogs.Add(blog);
                await context.SaveChangesAsync();

                var retrievedBlog = await blogService.GetBlogByIdAsync(blog.Id);
                Assert.NotNull(retrievedBlog);
                Assert.Equal("Blog 1", retrievedBlog?.Title);
            }
        }

        [Fact]
        public async Task UpdateBlog_ShouldModifyBlog()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var blogService = new BlogService(databaseService);

                var blog = new Blog { Title = "Old Title", Content = "Old Content" };
                context.Blogs.Add(blog);
                await context.SaveChangesAsync();

                blog.Title = "New Title";
                blog.Content = "New Content";
                await blogService.UpdateBlogAsync(blog);

                var updatedBlog = await context.Blogs.FindAsync(blog.Id);
                Assert.NotNull(updatedBlog);
                Assert.Equal("New Title", updatedBlog?.Title);
            }
        }

        [Fact]
        public async Task DeleteBlog_ShouldRemoveBlog()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var blogService = new BlogService(databaseService);

                var blog = new Blog { Title = "Blog to Delete", Content = "Content" };
                context.Blogs.Add(blog);
                await context.SaveChangesAsync();

                await blogService.DeleteBlogAsync(blog.Id);

                Assert.Empty(await context.Blogs.ToListAsync());
            }
        }
    }
}
