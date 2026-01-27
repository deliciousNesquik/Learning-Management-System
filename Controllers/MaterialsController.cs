using LMS.Interfaces;
using LMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController(IFileStorageService storageService, UserRequestContext userContext)
    : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (userContext.UserRole != "Administrator") return Unauthorized();
        
        if (file == null || file.Length == 0) 
            return BadRequest("Файл не предоставлен.");

        await using var stream = file.OpenReadStream();
        var fileKey = await storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

        // Возвращаем ключ файла, который нужно сохранить в БД
        return Ok(new { Key = fileKey });
    }
    
    [HttpGet]
    public IActionResult GetFileUrl(string fileKey)
    {
        if (userContext.UserRole != "Administrator" 
            || userContext.UserRole != "Moderator" 
            || userContext.UserRole != "Employee") return Unauthorized();
        
        if (string.IsNullOrEmpty(fileKey)) 
            return BadRequest("Ключ файла не указан.");
        
        var url = storageService.GetPresignedUrl(fileKey);
        
        return Ok(new { Url = url });
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(string fileKey)
    {
        if (userContext.UserRole != "Administrator")  return Unauthorized();
        
        if (string.IsNullOrEmpty(fileKey)) 
            return BadRequest("Ключ файла не указан.");

        await storageService.DeleteFileAsync(fileKey);
        
        return NoContent();
    }
}