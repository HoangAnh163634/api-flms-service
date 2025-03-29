using api_flms_service.Controllers;
using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace api_flms_service.Tests
{
    public class LoanControllerTests
    {
        private readonly Mock<ILoanService> _loanServiceMock;
        private readonly VnPayService _vnPayServiceStub;  // Sử dụng Stub thay vì Mock
        private readonly IOptions<LoanSettings> _loanSettingsMock;
        private readonly LoanController _controller;

        public LoanControllerTests()
        {
            _loanServiceMock = new Mock<ILoanService>();

            // Tạo một stub của VnPayService với các phương thức giả định
            _vnPayServiceStub = new VnPayService(); // Nếu có constructor, cần cung cấp dependency hợp lệ

            _loanSettingsMock = Options.Create(new LoanSettings());

            _controller = new LoanController(_loanServiceMock.Object, _vnPayServiceStub, _loanSettingsMock);
        }

        [Fact]
        public async Task GetLoan_ShouldReturnNotFound_WhenLoanDoesNotExist()
        {
            // Arrange
            _loanServiceMock.Setup(s => s.GetLoanByIdAsync(It.IsAny<int>())).ReturnsAsync((Loan)null);

            // Act
            var result = await _controller.GetLoan(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateLoan_ShouldReturnCreated_WhenLoanIsValid()
        {
            // Arrange
            var loan = new Loan { BookLoanId = 1 };
            _loanServiceMock.Setup(s => s.CreateLoanAsync(It.IsAny<Loan>())).ReturnsAsync(loan);

            // Act
            var result = await _controller.CreateLoan(loan);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(loan.BookLoanId, ((Loan)createdResult.Value).BookLoanId);
        }

        [Theory]
        [InlineData(1, true)]  // ✅ Loan hợp lệ
        [InlineData(-1, false)] // ❌ Loan ID không hợp lệ
        public async Task CreateLoan_ShouldHandleVariousInputs(int bookLoanId, bool expectedSuccess)
        {
            // Arrange
            var loan = new Loan { BookLoanId = bookLoanId };

            if (expectedSuccess)
            {
                _loanServiceMock.Setup(s => s.CreateLoanAsync(It.IsAny<Loan>())).ReturnsAsync(loan);
            }
            else
            {
                _loanServiceMock.Setup(s => s.CreateLoanAsync(It.IsAny<Loan>())).ThrowsAsync(new ArgumentException("Invalid Loan ID"));
            }

            // Act
            var result = await _controller.CreateLoan(loan);

            // Assert
            if (expectedSuccess)
            {
                var createdResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(loan.BookLoanId, ((Loan)createdResult.Value).BookLoanId);
            }
            else
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Invalid Loan ID", badRequestResult.Value);
            }
        }

        [Fact]
        public async Task CreateLoan_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var loan = new Loan { BookLoanId = 999 };
            _loanServiceMock.Setup(s => s.CreateLoanAsync(It.IsAny<Loan>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateLoan(loan);

            // Assert
            var badRequestResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Contains("Database error", badRequestResult.Value.ToString());
        }


        [Fact]
        public async Task CreateLoan_ShouldReturnBadRequest_WhenLoanIsNull()
        {
            // Act
            var result = await _controller.CreateLoan(null);

            // Assert
            var badRequestResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

    }
}

