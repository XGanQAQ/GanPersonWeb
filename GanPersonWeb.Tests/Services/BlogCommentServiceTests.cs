using GanPersonWeb.Data;
using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GanPersonWeb.Tests.Services
{
    public class BlogCommentServiceTests
    {
        private DbContextOptions<GanPersonDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddComment_ShouldAddComment()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var commentService = new BlogCommentService(databaseService);

                var comment = new Comment
                {
                    BlogId = 1,
                    Author = "TestUser",
                    Content = "This is a test comment."
                };

                await commentService.AddCommentAsync(comment);

                Assert.Single(await context.Comments.ToListAsync());
            }
        }

        [Fact]
        public async Task GetCommentsByBlogId_ShouldReturnCorrectComments()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var commentService = new BlogCommentService(databaseService);

                var comment1 = new Comment { BlogId = 1, Author = "A", Content = "C1" };
                var comment2 = new Comment { BlogId = 1, Author = "B", Content = "C2" };
                var comment3 = new Comment { BlogId = 2, Author = "C", Content = "C3" };

                context.Comments.AddRange(comment1, comment2, comment3);
                await context.SaveChangesAsync();

                var comments = await commentService.GetCommentsByBlogIdAsync(1);
                Assert.Equal(2, comments.Count);
                Assert.All(comments, c => Assert.Equal(1, c.BlogId));
            }
        }

        [Fact]
        public async Task DeleteComment_ShouldRemoveComment()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var commentService = new BlogCommentService(databaseService);

                var comment = new Comment { BlogId = 1, Author = "A", Content = "ToDelete" };
                context.Comments.Add(comment);
                await context.SaveChangesAsync();

                await commentService.DeleteCommentAsync(comment.Id);

                Assert.Empty(await context.Comments.ToListAsync());
            }
        }

        [Fact]
        public async Task GetCommentById_ShouldReturnCorrectComment()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var commentService = new BlogCommentService(databaseService);

                var comment = new Comment { BlogId = 1, Author = "A", Content = "FindMe" };
                context.Comments.Add(comment);
                await context.SaveChangesAsync();

                var found = await commentService.GetCommentByIdAsync(comment.Id);
                Assert.NotNull(found);
                Assert.Equal("FindMe", found?.Content);
            }
        }
    }
}