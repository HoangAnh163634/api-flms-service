using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using api_flms_service.Controllers;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace api_flms_service.Tests
{
    public class AuthorControllerSdkTests
    {
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly AuthorController _controller;

        public AuthorControllerSdkTests()
        {
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _controller = new AuthorController(null, _mockCloudinaryService.Object);  // We're only testing the Cloudinary part here
        }

        [Fact]
        public async Task UploadFile_Should_Call_CloudinaryService_UploadFileAsync()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("image.png");

            _mockCloudinaryService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("http://cloudinary.com/your_uploaded_file_url");

            // Act
            var result = await _controller.UploadFile(mockFile.Object);

            // Assert
            _mockCloudinaryService.Verify(service => service.UploadFileAsync(It.IsAny<IFormFile>()), Times.Once, "UploadFileAsync was not called exactly once.");
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Cast the response to dynamic
            dynamic response = okResult.Value;

            // Access the dynamic properties correctly
            Assert.Equal("http://cloudinary.com/your_uploaded_file_url", response.fileUrl);
        }

        [Fact]
        public async Task UploadFile_Should_Return_BadRequest_When_File_Is_Null()
        {
            // Act
            var result = await _controller.UploadFile(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic response = badRequestResult.Value;
            Assert.Equal("No file uploaded.", response.message);
        }

        [Fact]
        public async Task UploadFile_Should_Return_BadRequest_When_File_Format_Is_Invalid()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("invalidfile.txt");

            // Act
            var result = await _controller.UploadFile(mockFile.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic response = badRequestResult.Value;
            Assert.Equal("Invalid file format. Allowed formats: PNG, JPG, JPEG, GIF.", response.message);
        }
    }
}
