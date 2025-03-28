using api_flms_service.Entity;

namespace api_flms_service.Model
{
    // Tạo một DTO mới cho Category
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}