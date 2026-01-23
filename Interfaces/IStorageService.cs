namespace LMS.Interfaces;

public interface IStorageService
{
    ValueTask SetItemAsync<T>(string key, T value);
    ValueTask<T?> GetItemAsync<T>(string key);
    ValueTask RemoveItemAsync(string key);
}