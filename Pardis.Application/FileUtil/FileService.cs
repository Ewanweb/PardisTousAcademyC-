using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Pardis.Application.FileUtil;

 public class FileService : IFileService
    {
        private readonly string _rootPath;

        public FileService(IConfiguration configuration)
        {
            // Get the root path from configuration, fallback to current directory
            _rootPath = configuration["FileStorage:RootPath"] ?? Directory.GetCurrentDirectory();
        }
        public void DeleteDirectory(string directoryPath)
        {
            var fullPath = Path.Combine(_rootPath, directoryPath);
            if (Directory.Exists(fullPath))
                Directory.Delete(fullPath, true);
        }

        public void DeleteFile(string path, string fileName)
        {
            var filePath = Path.Combine(_rootPath, path, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_rootPath, filePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public async Task SaveFile(IFormFile file, string directoryPath)
        {
            if (file == null)
                throw new InvalidDataException("file is Null");

            var fileName = file.FileName;

            var folderName = Path.Combine(_rootPath, directoryPath.Replace("/", "\\"));
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            var path = Path.Combine(folderName, fileName);
            using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);
        }

        public async Task<string> SaveFileAndGenerateName(IFormFile? file, string directoryPath)
        {
            if (file == null)
                throw new InvalidDataException("file is Null");

            var fileName = file.FileName;

            fileName = Guid.NewGuid() + DateTime.Now.TimeOfDay.ToString()
                                          .Replace(":", "")
                                          .Replace(".", "") + Path.GetExtension(fileName);

            var folderName = Path.Combine(_rootPath, directoryPath.Replace("/", "\\"));
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            var path = Path.Combine(folderName, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }
    }