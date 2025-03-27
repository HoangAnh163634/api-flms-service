using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Notification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    public bool IsRead { get; set; }
}
