namespace FileStorageApi.Services
{
    public interface IFileStorageService
    {
        Task SaveFileAsync(string path, Stream content);
        Task<Stream> GetFileAsync(string path);
        Task<FileInfo?> GetFileInfoAsync(string path);
        Task DeleteFileAsync(string path);
        Task<IEnumerable<string?>> ListDirectoryAsync(string path);
        bool isDirectory(string path);
    }
}
