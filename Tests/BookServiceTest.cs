//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using api_flms_service.Entity;
//using api_flms_service.Service;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Xunit;

//public class BookServiceTests
//{
//    private async Task<AppDbContext> GetInMemoryDbContext()
//    {
//        var options = new DbContextOptionsBuilder<AppDbContext>()
//            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Tạo DB mới cho mỗi test
//            .Options;

//        var dbContext = new AppDbContext(options);
//        await dbContext.Database.EnsureCreatedAsync();

//        // Thêm dữ liệu giả
//        dbContext.Authors.Add(new Author { AuthorId = 1, Name = "Author Test" });
//        await dbContext.SaveChangesAsync();

//        return dbContext;
//    }

//    [Fact]
//    public async Task CreateBookAsync_ValidBook_ShouldReturnCreatedBook()
//    {
//        // Arrange
//        var dbContext = await GetInMemoryDbContext();
//        var bookService = new BookService(dbContext);

//        var newBook = new Book
//        {
//            Title = "Test Book",
//            AuthorId = 1,
//            ISBN = "1234567890",
//            AvailableCopies = 10
//        };

//        // Act
//        var createdBook = await bookService.CreateBookAsync(newBook);

//        // Assert
//        Assert.NotNull(createdBook);
//        Assert.Equal("Test Book", createdBook.Title);
//        Assert.Equal(1, createdBook.AuthorId);
//    }

//    [Fact]
//    public async Task CreateBookAsync_AuthorNotExists_ShouldThrowException()
//    {
//        // Arrange
//        var dbContext = await GetInMemoryDbContext();
//        var bookService = new BookService(dbContext);

//        var newBook = new Book
//        {
//            Title = "Test Book",
//            AuthorId = 99, // Author không tồn tại
//            ISBN = "1234567890",
//            AvailableCopies = 10
//        };

//        // Act & Assert
//        await Assert.ThrowsAsync<DbUpdateException>(() => bookService.CreateBookAsync(newBook));
//    }

//    [Fact]
//    public async Task CreateBookAsync_SaveChangesFails_ShouldThrowException()
//    {
//        // Arrange
//        var mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
//        mockDbContext.Setup(db => db.SaveChangesAsync(default)).ThrowsAsync(new Exception("Save failed"));

//        var bookService = new BookService(mockDbContext.Object);

//        var newBook = new Book
//        {
//            Title = "Test Book",
//            AuthorId = 1,
//            ISBN = "1234567890",
//            AvailableCopies = 10
//        };

//        // Act & Assert
//        await Assert.ThrowsAsync<Exception>(() => bookService.CreateBookAsync(newBook));
//    }
//}
