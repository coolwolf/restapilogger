using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

public sealed class RestApiLogger(string name, Func<RestApiLoggerConf> getCurrentConfig) : ILogger
{
    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri(getCurrentConfig().ApiHost)
    };

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;
    public bool IsEnabled(LogLevel logLevel) => getCurrentConfig().LogLevels.Contains(logLevel.ToString());
    private string ApiMethod => getCurrentConfig().ApiMethod;
    private string AuthType => getCurrentConfig().AuthType;
    private string AuthUser => getCurrentConfig().AuthUser;
    private string AuthPass => getCurrentConfig().AuthPass;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            Console.WriteLine(formatter(state, exception));
            return;
        }

        var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        if (!string.IsNullOrEmpty(version))
            version = version.Contains("+")
                ? version.Substring(0, version.IndexOf("+", StringComparison.Ordinal))
                : version;
        else version = "-1";
        var settings = new JsonSerializerOptions
        {
            Converters = { new SystemTextJsonExceptionConverter() },
            WriteIndented = true
        };
        var myJson = JsonSerializer.Serialize(new
        {
            LogSource = Assembly.GetEntryAssembly()?.GetName().Name,
            Environment.MachineName,
            AppVersion = version,
            LogLevel = logLevel,
            LogLevelDescription = logLevel.ToString(),
            EventId = eventId.Id,
            EventName = eventId.Name,
            Exception = exception,
            ClassName = name,
            LogContent = formatter(state, exception)
        },options:settings);
        using StringContent jsonContent = new(myJson, Encoding.UTF8, "application/json");
        if (_http.DefaultRequestHeaders.Authorization == null)
        {
            if (AuthType == "Basic")
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", CreateBasicAuthenticationHeader());
        }

        if (ApiMethod == "POST")
        {
            try
            {
                _ = _http.PostAsync(getCurrentConfig().ApiUrl, jsonContent).Result;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else if (ApiMethod == "GET")
        {
            try
            {
                _ = _http.GetAsync(getCurrentConfig().ApiUrl).Result;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private string CreateBasicAuthenticationHeader()
    {
        var builder = new StringBuilder().Append(AuthUser).Append(':').Append(AuthPass);
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(builder.ToString()));
        return credentials;
    }
}