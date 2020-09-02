using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFile(byte[] content, string extension, string containerName, string contentType);
        Task<string> EditFile(byte[] content, string extension, string containerName, string currentFileURI, string contentType);

        Task DeleteFile(string fileURI, string containerName);
    }
}