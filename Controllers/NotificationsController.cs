using Microsoft.AspNetCore.Mvc;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using System.Threading.Tasks;

namespace api_flms_service.Controllers
{
    [Route("api/v0/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/v0/notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        // GET: api/v0/notifications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotificationById(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        // POST: api/v0/notifications
        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdNotification = await _notificationService.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotification.Id }, createdNotification);
        }

        // PUT: api/v0/notifications/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Notification>> UpdateNotification(int id, Notification notification)
        {
            if (id != notification.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedNotification = await _notificationService.UpdateNotificationAsync(notification);
            if (updatedNotification == null)
            {
                return NotFound();
            }
            return Ok(updatedNotification);
        }

        // DELETE: api/v0/notifications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}