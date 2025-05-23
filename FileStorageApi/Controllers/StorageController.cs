using FileStorageApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageApi.Controllers
{
    [ApiController]
    [Route("/")]

    public class StorageController : ControllerBase
    {
        private readonly string _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

        private readonly IFileStorageService _fileStorageService;

        public StorageController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPut]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var fileName = Path.GetFileName(file.FileName); // обязательно безопасное имя
            var path = $"uploads/{fileName}"; // <== вот тут путь должен включать имя файла

            using (var stream = file.OpenReadStream())
            {
                await _fileStorageService.SaveFileAsync(path, stream);
            }

            return Ok("Файл загружен");

        }

        [HttpGet]
        public async Task<IActionResult> Get(string filePath)
        {
            if (_fileStorageService.isDirectory(filePath))
            {
                try
                {
                    var items = await _fileStorageService.ListDirectoryAsync(filePath);
                    return Ok(items);
                }
                catch
                {
                    return NotFound();
                }
            }
            else
            {
                try
                {
                    var stream = await _fileStorageService.GetFileAsync(filePath);
                    return File(stream, "application/octet-stream");
                }
                catch
                {
                    return NotFound();
                }
            }

        }

        [HttpHead]
        public async Task<IActionResult> Head(string filepath)
        {
            var info = await _fileStorageService.GetFileInfoAsync(filepath);
            if (info == null)
                return NotFound();

            Response.Headers["Content-Length"] = info.Length.ToString();
            Response.Headers["Last-Modified"] = info.LastWriteTimeUtc.ToString("R");
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string filepath)
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(filepath);
                return NoContent();
            }
            catch
            {
                return NotFound();
            }
        }

    }
}
