using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using api_flms_service.Controllers;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace api_flms_service.Tests
{
    public class AuthorControllerTests
    {
        private readonly Mock<IAuthorService> _mockAuthorService;
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly AuthorController _controller;

        public AuthorControllerTests()
        {
            _mockAuthorService = new Mock<IAuthorService>();
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _controller = new AuthorController(_mockAuthorService.Object, _mockCloudinaryService.Object);
        }

        [Fact]
        public async Task GetAllAuthors_ReturnsOkResult_WithAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { AuthorId = 1, Name = "Author 1" },
                new Author { AuthorId = 2, Name = "Author 2" }
            };
            _mockAuthorService.Setup(service => service.GetAllAuthorsAsync()).ReturnsAsync(authors);

            // Act
            var result = await _controller.GetAllAuthors();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAuthors = Assert.IsAssignableFrom<List<Author>>(okResult.Value);
            Assert.Equal(2, returnAuthors.Count);
        }

        [Fact]
        public async Task GetAuthorById_ReturnsOkResult_WithAuthor()
        {
            // Arrange
            var author = new Author { AuthorId = 1, Name = "Author 1" };
            _mockAuthorService.Setup(service => service.GetAuthorByIdAsync(1)).ReturnsAsync(author);

            // Act
            var result = await _controller.GetAuthorById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAuthor = Assert.IsType<Author>(okResult.Value);
            Assert.Equal("Author 1", returnAuthor.Name);
        }

        [Fact]
        public async Task GetAuthorById_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            _mockAuthorService.Setup(service => service.GetAuthorByIdAsync(It.IsAny<int>())).ReturnsAsync((Author)null);

            // Act
            var result = await _controller.GetAuthorById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Author not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task AddAuthor_ReturnsCreatedAtAction_WithAuthor()
        {
            // Arrange
            var author = new Author { Name = "New Author" };
            _mockAuthorService.Setup(service => service.AddAuthorAsync(author)).ReturnsAsync(new Author { AuthorId = 1, Name = "New Author" });

            // Act
            var result = await _controller.AddAuthor(author);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnAuthor = Assert.IsType<Author>(createdAtActionResult.Value);
            Assert.Equal("New Author", returnAuthor.Name);
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsOkResult_WithUpdatedAuthor()
        {
            // Arrange
            var author = new Author { AuthorId = 1, Name = "Updated Author" };
            _mockAuthorService.Setup(service => service.UpdateAuthorAsync(author)).ReturnsAsync(author);

            // Act
            var result = await _controller.UpdateAuthor(1, author);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedAuthor = Assert.IsType<Author>(okResult.Value);
            Assert.Equal("Updated Author", updatedAuthor.Name);
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            var author = new Author { AuthorId = 999, Name = "Non Existent Author" };
            _mockAuthorService.Setup(service => service.UpdateAuthorAsync(author)).ReturnsAsync((Author)null);

            // Act
            var result = await _controller.UpdateAuthor(999, author);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Author not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteAuthor_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            _mockAuthorService.Setup(service => service.DeleteAuthorAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAuthor(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            _mockAuthorService.Setup(service => service.DeleteAuthorAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAuthor(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Author not found.", notFoundResult.Value);
        }

        /*[Fact]
        public async Task UploadFile_ReturnsOkResult_WithFileUrl()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("image.png");

            _mockCloudinaryService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>())).ReturnsAsync("http://cloudinary.com/file_url");

            // Act
            var result = await _controller.UploadFile(mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<JsonResult>(okResult.Value);
            Assert.Equal("http://cloudinary.com/file_url", response.Value.fileUrl);
        }*/
/*
        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsNull()
        {
            // Act
            var result = await _controller.UploadFile(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<JsonResult>(badRequestResult.Value);
            Assert.Equal("No file uploaded.", response.message);
        }*/

        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenInvalidFileFormat()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("invalidfile.txt");

            // Act
            var result = await _controller.UploadFile(mockFile.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<JsonResult>(badRequestResult.Value);
            Assert.Equal("Invalid file format. Allowed formats: PNG, JPG, JPEG, GIF.", response.Value);
        }
    }
}
