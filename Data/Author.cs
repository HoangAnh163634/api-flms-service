using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Author
{
    public int Authorid { get; set; }

    public string? Biography { get; set; }

    public string? Cloudinaryid { get; set; }

    public string? Countryoforigin { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
