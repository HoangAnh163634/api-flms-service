using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Book
{
    public int Bookid { get; set; }

    public int Authorid { get; set; }

    public int Availablecopies { get; set; }

    public string Bookdescription { get; set; } = null!;

    public string Cloudinaryimageid { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public int Publicationyear { get; set; }

    public string Title { get; set; } = null!;

    public DateTime Borroweduntil { get; set; }

    public int Userid { get; set; }

    public string Imageurls { get; set; } = null!;

    public string Bookfileurl { get; set; } = null!;

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<Bookcategory> BookcategoryBookid2Navigations { get; set; } = new List<Bookcategory>();

    public virtual ICollection<Bookcategory> BookcategoryBooks { get; set; } = new List<Bookcategory>();

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
