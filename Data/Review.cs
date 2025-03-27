using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Review
{
    public int Reviewid { get; set; }

    public int Bookid { get; set; }

    public int Rating { get; set; }

    public DateTime? Reviewdate { get; set; }

    public string Reviewtext { get; set; } = null!;

    public int Userid { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
