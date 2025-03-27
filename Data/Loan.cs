using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Loan
{
    public int Bookloanid { get; set; }

    public int Bookid { get; set; }

    public DateTime? Loandate { get; set; }

    public DateTime? Returndate { get; set; }

    public int Userid { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
