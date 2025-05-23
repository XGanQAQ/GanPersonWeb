using GanPersonWeb.Services;
using GanPersonWeb.Data;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace GanPersonWeb.Test.Services
{
    public class DatabaseServiceTests
    {
        private readonly DatabaseService _service;

        public DatabaseServiceTests()
        {
            // 创建 InMemoryDatabase 的 DbContextOptions
            var options = new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            // 创建真实的 DbContext
            var context = new GanPersonDbContext(options);

            // 初始化 DatabaseService
            _service = new DatabaseService(context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var blog1 = new Blog { Id = 1, Title = "Test Blog 1" };
            var blog2 = new Blog { Id = 2, Title = "Test Blog 2" };

            // 添加测试数据
            var context = _service.GetType()
                .GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(_service) as GanPersonDbContext;

            context?.Blogs.AddRange(blog1, blog2);
            await context?.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync<Blog>();

            // Assert
            result.Should().NotBeNull(); // 使用 FluentAssertions 提供的扩展方法
            result.Should().HaveCount(2);
        }
    }
}


