namespace XafMaui.Services;

public static class ApiConfig
{
    // Android emulator: 10.0.2.2 maps to host localhost
    // iOS simulator: localhost works directly
    // Physical devices: use the actual machine IP
#if ANDROID
    public const string BaseUrl = "https://10.0.2.2:5001";
#else
    public const string BaseUrl = "https://localhost:5001";
#endif
}
