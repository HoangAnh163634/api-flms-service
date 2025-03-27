using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class User
{
    public int Userid { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phonenumber { get; set; } = null!;

    public DateTime? Registrationdate { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
