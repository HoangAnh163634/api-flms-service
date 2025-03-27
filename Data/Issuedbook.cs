using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Issuedbook
{
    public int Sno { get; set; }

    public string? Bookno { get; set; }

    public string? Bookname { get; set; }

    public string? Bookauthor { get; set; }

    public int Studentid { get; set; }

    public string? Status { get; set; }

    public DateTime? Issuedate { get; set; }
}
