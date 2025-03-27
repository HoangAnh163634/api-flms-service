using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Bookcategory
{
    public int Bookid { get; set; }

    public int Categoryid { get; set; }

    public int? Bookid2 { get; set; }

    public int? Categoryid2 { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Book? Bookid2Navigation { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Category? Categoryid2Navigation { get; set; }
}
