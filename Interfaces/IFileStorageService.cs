namespace LMS.Interfaces;

public interface IFileStorageService
{
    // Возвращает URL загруженного файла
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    
    // Удаление файла
    Task DeleteFileAsync(string fileKey);
    
    // Генерация временной ссылки (есть ограничение по времени)
    string GetPresignedUrl(string fileKey, double durationMinutes = 60);
}