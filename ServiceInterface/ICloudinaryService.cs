using CloudinaryDotNet.Actions;

namespace api_flms_service.ServiceInterface
{
    public interface ICloudinaryService
    {
        Task<UploadResult> UploadFileAsync(IFormFile file);
        Task<GetResourceResult> GetFileAsync(string publicId);
        Task<DeletionResult> DeleteFileAsync(string publicId);
    }
}
