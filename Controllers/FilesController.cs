using LMS.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers;

[ApiController]
[Route("api/")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _storageService;

    public FilesController(IFileStorageService storageService)
    {
        _storageService = storageService;
    }
    
    [HttpPost("upload-material")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) 
            return BadRequest("Файл не предоставлен.");

        using var stream = file.OpenReadStream();
        var fileKey = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

        // Возвращаем ключ файла, который нужно сохранить в БД
        return Ok(new { Key = fileKey });
    }
    
    [HttpGet("get-material/{*fileKey}")]
    public IActionResult GetFileUrl(string fileKey)
    {
        if (string.IsNullOrEmpty(fileKey)) 
            return BadRequest("Ключ файла не указан.");
        
        var url = _storageService.GetPresignedUrl(fileKey);
        
        return Ok(new { Url = url });
    }
    
    [HttpDelete("remove-material/{*fileKey}")]
    public async Task<IActionResult> Delete(string fileKey)
    {
        if (string.IsNullOrEmpty(fileKey)) 
            return BadRequest("Ключ файла не указан.");

        await _storageService.DeleteFileAsync(fileKey);
        
        return NoContent(); // 204 Success
    }
}