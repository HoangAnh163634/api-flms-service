using api_flms_service.ServiceInterface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using api_flms_service.Model;

namespace api_flms_service.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            var account = new Account(
                cloudinarySettings.Value.CloudName,
                cloudinarySettings.Value.ApiKey,
                cloudinarySettings.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            var isImage = extension is ".png" or ".jpg"; // Kiểm tra có phải hình ảnh không

            // Chọn đúng loại UploadParams
            var uploadParams = isImage
                ? new ImageUploadParams() // Dùng ImageUploadParams cho ảnh
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                }
                : new RawUploadParams() // Dùng RawUploadParams cho các file khác
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.AbsoluteUri; // Trả về URL file đã upload
        }



        // Get file
        public async Task<GetResourceResult> GetFileAsync(string publicId)
        {
            var getResult = await _cloudinary.GetResourceAsync(publicId);
            return getResult;
        }

        // Delete file
        public async Task<DeletionResult> DeleteFileAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
            return deleteResult;
        }
    }
}
