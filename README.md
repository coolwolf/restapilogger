# restapilogger
NetCore Rest Api Logger


This project helps standard .Net Core ILogger send logs to Web Api target.

# Reason

We were using [Parseable](https://www.parseable.com/) as logs target.
Because we use "Basic Authentication" to post logs, none of [ILogger](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0) and [Serilog](https://serilog.net/) helped.
Implementing "Custom HttpClient" for "Serilog.Sinks.Http" was the first thing we tried. But "Adding Custom Logger" will decrease code and needed extensions we thought.

# Status

Sending logs to Parseable Api Endpoint using POST and Basic Authentication is working well.

# Future

We may improve this code to send logs more structured and possibly different targets.
Because this is just an "Http Target Helper" for standart .Net Core ILogger.
So it may be useful to collect logs with many different log servers.

# Usage

First define settings in appsettings.json

```
"Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "RestApiLogger":{
      "LogLevels":"Debug,Warning,Error,Trace",
      "ApiHost": "http://192.168.0.100:8000",
      "ApiUrl":"/api/v1/logstream/{mystreamname}",
      "ApiMethod":"POST",
      "AuthType":"Basic",
      "AuthUser":"{mystreamuser}",
      "AuthPass":"{mystreampass}"
    }
  }
```

Just after
```
var builder = WebApplication.CreateBuilder(args);
```
in Program.cs. Add
```
builder.Logging.ClearProviders();
builder.Logging.AddRestApiLogger();
```

If you want to log from Program.cs

```
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogDebug(1, "This is Debug Log.");
logger.LogInformation(3, "This is info Log");
logger.LogWarning(5, "This is warning log.");
logger.LogError(7, "This is Error Log.");
logger.LogTrace(5, "== 120.");
```

If you want to log from Class/Controller:
```
public class TestController(ILogger<TestController> log, IMyMethodInterface mmi) : ControllerBase
{
    [HttpGet]
    public async Task<string> ListAllItems()
    {
        log.LogDebug(1,"Starting list all items");
        try {
            var myResult= await mmi.ListAll();
            log.LogDebug(1,"{count} items listed.",myResult.Count);
        }catch(Exception ex){
            log.LogError(ex, "Test.Get");
        }
    }
}
```


# Helping

You are always welcome to use and improve code.