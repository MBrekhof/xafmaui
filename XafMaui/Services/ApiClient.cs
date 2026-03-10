using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XafMaui.Services;

public class ApiClient
{
    readonly HttpClient _http;
    readonly AuthService _auth;
    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApiClient(HttpClient http, AuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    async Task EnsureAuthHeaderAsync()
    {
        var token = await _auth.GetTokenAsync();
        if (token != null)
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<T>> GetListAsync<T>(string odataPath, string? queryParams = null)
    {
        await EnsureAuthHeaderAsync();
        var url = $"{ApiConfig.BaseUrl}/api/odata/{odataPath}";
        if (queryParams != null)
            url += $"?{queryParams}";

        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ODataResponse<T>>(JsonOptions);
        return result?.Value ?? [];
    }

    public async Task<T?> PostAsync<T>(string odataPath, T entity)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/odata/{odataPath}", entity, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<T?> PostAsync<T>(string odataPath, object payload)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/odata/{odataPath}", payload, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task PutAsync<T>(string odataPath, int id, T entity)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"{ApiConfig.BaseUrl}/api/odata/{odataPath}/{id}", entity, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task<byte[]> GetBytesAsync(string relativePath)
    {
        await EnsureAuthHeaderAsync();
        var url = $"{ApiConfig.BaseUrl}/api/{relativePath}";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task DeleteAsync(string odataPath, int id)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.DeleteAsync($"{ApiConfig.BaseUrl}/api/odata/{odataPath}/{id}");
        response.EnsureSuccessStatusCode();
    }
}

public class ODataResponse<T>
{
    public List<T> Value { get; set; } = [];
}
