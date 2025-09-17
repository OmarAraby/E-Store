using Microsoft.AspNetCore.Http;

namespace Estore.Application.Utiles.HandleFiles
{
    public record FileUploadRequest(IFormFile File);

}
