using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_flms_service.Pages.Notifications
{
    public class ManageModel : PageModel
    {
        private readonly INotificationService _notificationService;

        public ManageModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IEnumerable<Notification> Notifications { get; set; } = new List<Notification>();

        [BindProperty]
        public Notification NewNotification { get; set; } = new Notification();

        [BindProperty]
        public Notification EditNotification { get; set; } = new Notification();

        public async Task OnGetAsync()
        {
            try
            {
                Notifications = await _notificationService.GetAllNotificationsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnGetAsync: {ex.Message}");
                Notifications = new List<Notification>();
            }
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Console.WriteLine("OnPostCreateAsync called");
            Console.WriteLine($"NewNotification: Title={NewNotification.Title}, Content={NewNotification.Content}, Type={NewNotification.Type}");

            try
            {
                NewNotification.CreatedAt = DateTime.UtcNow; // Sửa: Dùng Kind=Utc
                NewNotification.IsRead = false;
                await _notificationService.CreateNotificationAsync(NewNotification);
                TempData["SuccessMessage"] = "Notification created successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating notification: {ex.Message}";
                Notifications = await _notificationService.GetAllNotificationsAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            Console.WriteLine("OnPostUpdateAsync called");
            Console.WriteLine($"EditNotification: Id={EditNotification.Id}, Title={EditNotification.Title}, Content={EditNotification.Content}, Type={EditNotification.Type}, IsRead={EditNotification.IsRead}");

            try
            {
                var updatedNotification = await _notificationService.UpdateNotificationAsync(EditNotification);
                if (updatedNotification == null)
                {
                    TempData["ErrorMessage"] = "Notification not found.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Notification updated successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating notification: {ex.Message}";
                Notifications = await _notificationService.GetAllNotificationsAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var result = await _notificationService.DeleteNotificationAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Notification not found.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Notification deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting notification: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
