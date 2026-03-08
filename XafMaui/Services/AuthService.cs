using System.Net.Http.Json;

namespace XafMaui.Services;

public class AuthService
{
    readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> LoginAsync(string userName, string password)
    {
        var response = await _http.PostAsJsonAsync(
            $"{ApiConfig.BaseUrl}/api/Authentication/Authenticate",
            new { userName, password });

        if (!response.IsSuccessStatusCode)
            return null;

        var token = await response.Content.ReadAsStringAsync();
        await SecureStorage.SetAsync("jwt_token", token);
        return token;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync("jwt_token");
    }

    public void Logout()
    {
        SecureStorage.Remove("jwt_token");
    }
}
