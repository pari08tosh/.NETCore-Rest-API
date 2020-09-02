using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MoviesAPI.Services
{
    public class InAppStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InAppStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string fileURI, string containerName)
        {
            if (fileURI != null)
            {
                var fileName = Path.GetFileName(fileURI);
                var filePath = Path.Join(_env.WebRootPath, containerName, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string currentFileURI, string contentType)
        {
            await DeleteFile(currentFileURI, containerName);
            return await SaveFile(content, extension, containerName, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";

            string folder = Path.Combine(_env.WebRootPath, containerName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var savingPath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(savingPath, content);

            var currentUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var pathToFile = Path.Combine(currentUrl, containerName, fileName).Replace("\\", "/");
            return pathToFile;
        }
    }
}