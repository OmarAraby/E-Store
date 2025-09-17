using Microsoft.AspNetCore.Http;

namespace Estore.Application.Utiles.HandleFiles
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadFileAsync(IFormFile file);
    }
}
