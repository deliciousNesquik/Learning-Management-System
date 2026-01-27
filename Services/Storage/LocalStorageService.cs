using System.Text.Json;
using LMS.Interfaces;
using Microsoft.JSInterop;

namespace LMS.Services.Storage;

public class LocalStorageService (IJSRuntime jsRuntime) : IStorageService
{
    /// <summary>
    /// Позволяет установить структуру данных типа (ключ: значение)
    /// в локальное хранилище браузера LocalStorage.
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="value">Значение</param>
    /// <typeparam name="T">Позволяет установить любой тип данных для значения.</typeparam>
    public async ValueTask SetItemAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch (NotSupportedException) { }
    }

    /// <summary>
    /// Позволяет получить ту структуру данных типа (ключ: значение)
    /// которое было ранее установлено в локальное хранилище браузера LocalStorage.
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <typeparam name="T">Позволяет установить любой тип данных для значения.</typeparam>
    /// <returns>Значение для ключа</returns>
    public async ValueTask<T?> GetItemAsync<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        if (string.IsNullOrEmpty(json)) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Позволяет удалить ту структуру данных типа (ключ: значение)
    /// которая была ранее установлена в локальное хранилище браузера LocalStorage.
    /// </summary>
    /// <param name="key">Ключ</param>
    public async ValueTask RemoveItemAsync(string key) => await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
}