using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; } = null!;
    public bool IsRead { get; set; }
}
