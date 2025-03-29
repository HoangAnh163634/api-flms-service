using System;
using System.Threading.Tasks;
using api_flms_service.Controllers;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api_flms_service.Tests
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _controller = new CategoryController(_categoryServiceMock.Object);
        }

        [Theory]
        [InlineData("Technology")] // Hợp lệ
        [InlineData("Science")]    // Hợp lệ
        [InlineData("")]           // Không hợp lệ (Tên rỗng)
        [InlineData(null)]          // Không hợp lệ (Tên null)
        public async Task AddCategory_ValidationCases_ReturnsExpectedResult(string categoryName)
        {
            // Arrange
            var newCategory = new Category { CategoryId = 3, CategoryName = categoryName };

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                _categoryServiceMock.Setup(service => service.AddCategoryAsync(newCategory))
                                    .ThrowsAsync(new ArgumentException("Category name is required"));
            }
            else
            {
                _categoryServiceMock.Setup(service => service.AddCategoryAsync(It.IsAny<Category>()))
                                    .ReturnsAsync(newCategory);
            }

            // Act
            var result = await _controller.AddCategory(newCategory);

            // Assert
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
            else
            {
                var createdResult = Assert.IsType<CreatedAtActionResult>(result);
                var returnedCategory = Assert.IsType<Category>(createdResult.Value);
                Assert.Equal(categoryName, returnedCategory.CategoryName);
            }
        }
    }
}
