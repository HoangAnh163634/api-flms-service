using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Controllers
{
    [ApiController]
    [Route("api/v0/issued-books")]
    public class IssuedBookController : ControllerBase
    {
        private readonly IIssuedBookService _issuedBookService;

        public IssuedBookController(IIssuedBookService issuedBookService)
        {
            _issuedBookService = issuedBookService;
        }

        /// <summary>
        /// Get all issued books
        /// </summary>
        /// <returns>List of issued books</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllIssuedBooks()
        {
            try
            {
                var issuedBooks = await _issuedBookService.GetAllIssuedBooksAsync();
                return Ok(issuedBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving issued books.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get an issued book by ID
        /// </summary>
        /// <param name="id">Issued book ID</param>
        /// <returns>Issued book details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIssuedBookById(int id)
        {
            try
            {
                var issuedBook = await _issuedBookService.GetIssuedBookByIdAsync(id);
                if (issuedBook == null)
                {
                    return NotFound(new { message = "Issued book not found." });
                }

                return Ok(issuedBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the issued book.", details = ex.Message });
            }
        }

        /// <summary>
        /// Add a new issued book
        /// </summary>
        /// <param name="issuedBook">Issued book object</param>
        /// <returns>Created issued book</returns>
        [HttpPost]
        public async Task<IActionResult> AddIssuedBook([FromBody] IssuedBook issuedBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdIssuedBook = await _issuedBookService.AddIssuedBookAsync(issuedBook);
                return CreatedAtAction(nameof(GetIssuedBookById), new { id = createdIssuedBook.SNo }, createdIssuedBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the issued book.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing issued book
        /// </summary>
        /// <param name="issuedBook">Updated issued book object</param>
        /// <returns>Updated issued book</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateIssuedBook([FromBody] IssuedBook issuedBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingIssuedBook = await _issuedBookService.GetIssuedBookByIdAsync(issuedBook.SNo);
                if (existingIssuedBook == null)
                {
                    return NotFound(new { message = "Issued book not found." });
                }

                var updatedIssuedBook = await _issuedBookService.UpdateIssuedBookAsync(issuedBook);
                return Ok(updatedIssuedBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the issued book.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete an issued book by ID
        /// </summary>
        /// <param name="id">Issued book ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssuedBook(int id)
        {
            try
            {
                var success = await _issuedBookService.DeleteIssuedBookAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Issued book not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the issued book.", details = ex.Message });
            }
        }
    }
}
