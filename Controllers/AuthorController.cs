using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Controllers
{
    [ApiController]
    [Route("api/v0/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthorController(IAuthorService authorService, ICloudinaryService cloudinaryService)
        {
            _authorService = authorService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound("Author not found.");
            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] Author author)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(author.Name))
            {
                return BadRequest("Invalid data. Author name is required.");
            }
            var createdAuthor = await _authorService.AddAuthorAsync(author);
            return CreatedAtAction(nameof(GetAuthorById), new { id = createdAuthor.AuthorId }, createdAuthor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author author)
        {
            if (id != author.AuthorId)
            {
                return BadRequest("Author ID mismatch.");
            }
            if (!ModelState.IsValid || string.IsNullOrEmpty(author.Name))
            {
                return BadRequest("Invalid data. Author name is required.");
            }
            var updatedAuthor = await _authorService.UpdateAuthorAsync(author);
            if (updatedAuthor == null) return NotFound("Author not found.");
            return Ok(updatedAuthor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var result = await _authorService.DeleteAuthorAsync(id);
            if (!result) return NotFound("Author not found.");
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            var allowedExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Invalid file format. Allowed formats: PNG, JPG, JPEG, GIF." });
            }

            try
            {
                var uploadResult = await _cloudinaryService.UploadFileAsync(file);
                return Ok(new { fileUrl = uploadResult });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading file to Cloudinary", details = ex.Message });
            }
        }
    }
}