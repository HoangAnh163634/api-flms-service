using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Controllers
{
    [ApiController]
    [Route("api/v0/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        /// <summary>
        /// Get author by ID
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Author object</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound("Author not found.");
            return Ok(author);
        }

        /// <summary>
        /// Add a new author
        /// </summary>
        /// <param name="author">Author object</param>
        /// <returns>Created author</returns>
        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] Author author)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data.");
            var createdAuthor = await _authorService.AddAuthorAsync(author);
            return CreatedAtAction(nameof(GetAuthorById), new { id = createdAuthor.AuthorId }, createdAuthor);
        }

        /// <summary>
        /// Update an author
        /// </summary>
        /// <param name="author">Updated author object</param>
        /// <returns>Updated author</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor([FromBody] Author author)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data.");
            var updatedAuthor = await _authorService.UpdateAuthorAsync(author);
            if (updatedAuthor == null) return NotFound("Author not found.");
            return Ok(updatedAuthor);
        }

        /// <summary>
        /// Delete an author by ID
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Success or failure</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var result = await _authorService.DeleteAuthorAsync(id);
            if (!result) return NotFound("Author not found.");
            return NoContent();
        }
    }
}
