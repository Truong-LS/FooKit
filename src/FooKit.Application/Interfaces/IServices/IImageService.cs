using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
