using GanPersonWeb.Controllers;
using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using GanPersonWeb.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace GanPersonWeb.Test.Controllers
{
    public class PictureBedControllerTests
    {
        private readonly DatabaseService _dbService;
        private readonly PictureBedService _pictureBedService;
        private readonly PictureBedController _controller;
        private readonly string _testRootPath = Path.Combine(Path.GetTempPath(), "PictureBedControllerTestUploads");

        public PictureBedControllerTests()
        {
            var options = new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase("PictureBedControllerTestDb")
                .Options;
            var context = new GanPersonDbContext(options);
            _dbService = new DatabaseService(context);

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(_testRootPath);

            _pictureBedService = new PictureBedService(_dbService, envMock.Object);
            _controller = new PictureBedController(_pictureBedService);
        }

        [Fact]
        public async Task Upload_ShouldReturnOk_WhenFileIsValid()
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

            // Act
            var result = await _controller.Upload(fileMock.Object,"test.png","desc", "tag1,tag2");

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var image = okResult!.Value as Image;
            image.Should().NotBeNull();
            image!.Description.Should().Be("desc");
        }

        [Fact]
        public async Task Upload_ShouldReturnBadRequest_WhenFileIsNull()
        {
            var result = await _controller.Upload(null!,null!, null, null);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task List_ShouldReturnImages()
        {
            // Arrange
            await _dbService.AddAsync(new Image { Url = "/uploads/test.png", Description = "desc", Tags = "tag" });

            // Act
            var result = await _controller.List(_dbService);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var images = okResult!.Value as List<Image>;
            images.Should().NotBeNull();
            images!.Count.Should().BeGreaterThan(0);
        }
    }
}
