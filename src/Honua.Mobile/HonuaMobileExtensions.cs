using Honua.Mobile.Controls;
using Honua.Mobile.Services;
using Honua.Mobile.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Honua.Mobile;

/// <summary>
/// Extension methods for configuring Honua Mobile SDK in MAUI applications
/// </summary>
public static class HonuaMobileExtensions
{
    /// <summary>
    /// Add Honua Mobile SDK to the MAUI app
    /// </summary>
    /// <param name="builder">MAUI app builder</param>
    /// <returns>MAUI app builder for chaining</returns>
    public static MauiAppBuilder UseHonuaMobile(this MauiAppBuilder builder)
    {
        return builder.UseHonuaMobile(config => { });
    }

    /// <summary>
    /// Add Honua Mobile SDK to the MAUI app with configuration
    /// </summary>
    /// <param name="builder">MAUI app builder</param>
    /// <param name="configure">Configuration action</param>
    /// <returns>MAUI app builder for chaining</returns>
    public static MauiAppBuilder UseHonuaMobile(this MauiAppBuilder builder, Action<HonuaMobileConfiguration> configure)
    {
        var config = new HonuaMobileConfiguration();
        configure(config);

        // Register configuration
        builder.Services.AddSingleton(config);

        // Register MAUI handlers for cross-platform controls
        builder.ConfigureFonts(fonts => { });

        // Configure platform-specific handlers
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.AddHandler<HonuaMapView, Platforms.Android.HonuaMapViewHandler>();
            handlers.AddHandler<HonuaCameraView, Platforms.Android.HonuaCameraViewHandler>();
            handlers.AddHandler<HonuaFormView, Platforms.Android.HonuaFormViewHandler>();
#elif IOS
            handlers.AddHandler<HonuaMapView, Platforms.iOS.HonuaMapViewHandler>();
            handlers.AddHandler<HonuaCameraView, Platforms.iOS.HonuaCameraViewHandler>();
            handlers.AddHandler<HonuaFormView, Platforms.iOS.HonuaFormViewHandler>();
#elif WINDOWS
            handlers.AddHandler<HonuaMapView, Platforms.Windows.HonuaMapViewHandler>();
            handlers.AddHandler<HonuaCameraView, Platforms.Windows.HonuaCameraViewHandler>();
            handlers.AddHandler<HonuaFormView, Platforms.Windows.HonuaFormViewHandler>();
#endif
        });

        return builder;
    }

    /// <summary>
    /// Add Honua Mobile services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configure">Configuration action</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddHonuaMobile(this IServiceCollection services, Action<HonuaMobileConfiguration> configure)
    {
        var config = new HonuaMobileConfiguration();
        configure(config);

        // Register configuration
        services.AddSingleton(config);

        // Register core services
        services.AddSingleton<ILocationService, LocationService>();
        services.AddSingleton<IOfflineStorage, OfflineStorageService>();
        services.AddSingleton<IOfflineSyncService, OfflineSyncService>();

        // Register HTTP client for server communication
        services.AddHttpClient("HonuaAPI", client =>
        {
            if (!string.IsNullOrEmpty(config.ServerUrl))
            {
                client.BaseAddress = new Uri(config.ServerUrl);
            }

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                client.DefaultRequestHeaders.Add("X-API-Key", config.ApiKey);
            }

            client.DefaultRequestHeaders.Add("User-Agent", $"Honua-Mobile-SDK/{typeof(HonuaMobileExtensions).Assembly.GetName().Version}");
        });

        // Register gRPC client if configured
        if (!string.IsNullOrEmpty(config.ServerUrl))
        {
            services.AddGrpcClient<global::Grpc.Net.ClientFactory.GrpcClientFactory>(options =>
            {
                options.Address = new Uri(config.ServerUrl);
            });
        }

        return services;
    }
}

/// <summary>
/// Configuration options for Honua Mobile SDK
/// </summary>
public class HonuaMobileConfiguration
{
    /// <summary>
    /// URL of the Honua server
    /// </summary>
    public string? ServerUrl { get; set; }

    /// <summary>
    /// API key for server authentication
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Enable offline synchronization
    /// </summary>
    public bool EnableOfflineSync { get; set; } = true;

    /// <summary>
    /// Enable background location tracking
    /// </summary>
    public bool EnableBackgroundLocation { get; set; } = false;

    /// <summary>
    /// Default location accuracy
    /// </summary>
    public LocationAccuracy DefaultLocationAccuracy { get; set; } = LocationAccuracy.Medium;

    /// <summary>
    /// Cache size limit in bytes
    /// </summary>
    public long CacheSizeLimit { get; set; } = 100 * 1024 * 1024; // 100 MB

    /// <summary>
    /// Auto-sync options
    /// </summary>
    public AutoSyncOptions? AutoSyncOptions { get; set; }

    /// <summary>
    /// Logging configuration
    /// </summary>
    public LoggingConfiguration Logging { get; set; } = new();

    /// <summary>
    /// Security configuration
    /// </summary>
    public SecurityConfiguration Security { get; set; } = new();

    /// <summary>
    /// Performance configuration
    /// </summary>
    public PerformanceConfiguration Performance { get; set; } = new();
}

/// <summary>
/// Logging configuration
/// </summary>
public class LoggingConfiguration
{
    /// <summary>
    /// Enable debug logging
    /// </summary>
    public bool EnableDebugLogging { get; set; } = false;

    /// <summary>
    /// Log level for Honua SDK operations
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Enable performance logging
    /// </summary>
    public bool EnablePerformanceLogging { get; set; } = false;

    /// <summary>
    /// Log to console
    /// </summary>
    public bool LogToConsole { get; set; } = true;

    /// <summary>
    /// Log to file
    /// </summary>
    public bool LogToFile { get; set; } = false;

    /// <summary>
    /// Log file path (if LogToFile is enabled)
    /// </summary>
    public string? LogFilePath { get; set; }
}

/// <summary>
/// Security configuration
/// </summary>
public class SecurityConfiguration
{
    /// <summary>
    /// Enable certificate pinning
    /// </summary>
    public bool EnableCertificatePinning { get; set; } = false;

    /// <summary>
    /// Pinned certificate thumbprints
    /// </summary>
    public List<string> PinnedCertificates { get; set; } = new();

    /// <summary>
    /// Enable data encryption at rest
    /// </summary>
    public bool EnableDataEncryption { get; set; } = true;

    /// <summary>
    /// Require secure connection (HTTPS/TLS)
    /// </summary>
    public bool RequireSecureConnection { get; set; } = true;

    /// <summary>
    /// Enable request signing
    /// </summary>
    public bool EnableRequestSigning { get; set; } = false;

    /// <summary>
    /// Request signing key
    /// </summary>
    public string? RequestSigningKey { get; set; }
}

/// <summary>
/// Performance configuration
/// </summary>
public class PerformanceConfiguration
{
    /// <summary>
    /// Maximum concurrent sync operations
    /// </summary>
    public int MaxConcurrentSyncOperations { get; set; } = 3;

    /// <summary>
    /// Network timeout for API calls
    /// </summary>
    public TimeSpan NetworkTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Batch size for bulk operations
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Enable request compression
    /// </summary>
    public bool EnableRequestCompression { get; set; } = true;

    /// <summary>
    /// Enable response caching
    /// </summary>
    public bool EnableResponseCaching { get; set; } = true;

    /// <summary>
    /// Cache expiry time
    /// </summary>
    public TimeSpan CacheExpiryTime { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Enable image compression for attachments
    /// </summary>
    public bool EnableImageCompression { get; set; } = true;

    /// <summary>
    /// Image compression quality (0-100)
    /// </summary>
    public int ImageCompressionQuality { get; set; } = 80;
}

/// <summary>
/// Log levels for Honua SDK
/// </summary>
public enum LogLevel
{
    None,
    Error,
    Warning,
    Information,
    Debug,
    Trace
}