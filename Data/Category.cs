using System;
using System.Collections.Generic;

namespace api_flms_service.Data;

public partial class Category
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; } = null!;

    public virtual ICollection<Bookcategory> BookcategoryCategories { get; set; } = new List<Bookcategory>();

    public virtual ICollection<Bookcategory> BookcategoryCategoryid2Navigations { get; set; } = new List<Bookcategory>();
}
