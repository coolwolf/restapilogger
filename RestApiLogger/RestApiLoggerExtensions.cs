using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

public static class RestApiLoggerExtensions
{
    public static ILoggingBuilder AddRestApiLogger(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, RestApiLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<RestApiLoggerConf, RestApiLoggerProvider>(builder.Services);
        return builder;
    }

    public static ILoggingBuilder AddRestApiLogger(this ILoggingBuilder builder,Action<RestApiLoggerConf> configure)
    {
        builder.AddRestApiLogger();
        builder.Services.Configure(configure);
        return builder;
    }
}