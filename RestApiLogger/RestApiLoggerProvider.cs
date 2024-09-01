using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Options;

[UnsupportedOSPlatform("browser")]
[ProviderAlias("RestApiLogger")]
public sealed class RestApiLoggerProvider : ILoggerProvider
{
    private readonly IDisposable? _onChangeToken;
    private RestApiLoggerConf _currentConfig;
    private readonly ConcurrentDictionary<string, RestApiLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
    public RestApiLoggerProvider(IOptionsMonitor<RestApiLoggerConf> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }
    public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new RestApiLogger(name, GetCurrentConfig));
    private RestApiLoggerConf GetCurrentConfig() => _currentConfig;
    public void Dispose()
    {
        _loggers.Clear();
        _onChangeToken?.Dispose();
    }
}
