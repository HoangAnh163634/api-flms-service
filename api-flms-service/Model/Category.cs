using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class Category
    {
        [Key]
        public int CatId { get; set; } // Primary Key
        public string CatName { get; set; } = null!;
    }

}
