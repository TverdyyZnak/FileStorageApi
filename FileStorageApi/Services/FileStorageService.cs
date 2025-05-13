
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

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            using(var fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write)) 
            {
                await fs.CopyToAsync(content);
            }
        }
    }
}
