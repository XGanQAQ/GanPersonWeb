using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using GanPersonWeb.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace GanPersonWeb.Test.Services
{
    public class PictureBedServiceTests
    {
        private readonly DatabaseService _dbService;
        private readonly string _testRootPath = Path.Combine(Path.GetTempPath(), "PictureBedTestUploads");

        public PictureBedServiceTests()
        {
            var options = new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase("PictureBedTestDb")
                .Options;
            var context = new GanPersonDbContext(options);
            _dbService = new DatabaseService(context);

            if (!Directory.Exists(_testRootPath))
                Directory.CreateDirectory(_testRootPath);
        }

        [Fact]
        public async Task SaveImageAsync_ShouldSaveFileAndDbRecord()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Fake image content";
            var fileName = "test.png";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns<Stream, CancellationToken>((stream, token) =>
            {
                ms.Position = 0;
                return ms.CopyToAsync(stream, token);
            });

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(_testRootPath);

            var service = new PictureBedService(_dbService, envMock.Object);

            // Act
            var image = await service.SaveImageAsync(fileMock.Object, "desc", "tag1,tag2");

            // Assert
            image.Should().NotBeNull();
            image!.Description.Should().Be("desc");
            image.Tags.Should().Be("tag1,tag2");

            var path = $"{_testRootPath}{image.Url}";
            File.Exists(path).Should().BeTrue();
        }

        [Fact]
        public async Task SaveImageAsync_ShouldReturnNull_WhenFileIsNull()
        {
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(_testRootPath);
            var service = new PictureBedService(_dbService, envMock.Object);

            var result = await service.SaveImageAsync(null!);

            result.Should().BeNull();
        }
    }
}