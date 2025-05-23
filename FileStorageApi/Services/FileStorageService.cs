
namespace FileStorageApi.Services
{
    public class FileStorageService: IFileStorageService
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

        public FileStorageService() 
        {
            Directory.CreateDirectory(_filePath);
        }

        private string GetFullPath(string path)
        {
            return Path.Combine(_filePath, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        }



        public async Task DeleteFileAsync(string path)
        {
            var fullPath = GetFullPath(path);

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
            else if (File.Exists(fullPath)) 
            {
                File.Delete(path);
            }
            else
            {
                throw new FileNotFoundException();
            }

            await Task.CompletedTask;
        }

        public async Task<Stream> GetFileAsync(string path)
        {
            var fullPath = GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException();
            }

            return await Task.FromResult(File.OpenRead(fullPath));
        }

        public async Task<FileInfo?> GetFileInfoAsync(string path)
        {
            var fullPath = GetFullPath(path);
            var info = new FileInfo(fullPath);
            return await Task.FromResult(info.Exists ? info : null);
        }

        public bool isDirectory(string path)
        {
            var fullPath = GetFullPath(path);
            return Directory.Exists(fullPath);
        }

        public async Task<IEnumerable<string?>> ListDirectoryAsync(string path)
        {
            var fullPath = GetFullPath(path);
            if (!Directory.Exists(fullPath))
            {
                throw new DirectoryNotFoundException();
            }

            var entries = Directory.GetFileSystemEntries(fullPath).Select(Path.GetFileName);

            return await Task.FromResult(entries);
        }

        public async Task SaveFileAsync(string path, Stream content)
        {
            var fullPath = GetFullPath(path);

            if (Directory.Exists(fullPath))
            {
                throw new UnauthorizedAccessException("Нельзя сохранить файл по пути, указывающему на директорию.");
            }

            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await content.CopyToAsync(fs);
            }
        }
    }
}
