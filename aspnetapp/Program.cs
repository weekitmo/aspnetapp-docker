using Aspnetapp.Models;
using EasyCaching.Core;
using EasyCaching.Core.Configurations;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
string providerSerializerName = "mymsgpack";
builder.Services.AddEasyCaching(options =>
{
  options.UseRedis(config =>
    {
      var host = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
      var port = 6379;
      var result = int.TryParse(Environment.GetEnvironmentVariable("REDIS_PORT"), out port);
      if (!result)
      {
        port = 6379;
      }
      Console.WriteLine($"Redis: {host}:{port}/{result}");
      // config.SerializerName = providerSerializerName;
      config.DBConfig.Endpoints.Add(new ServerEndPoint(host, port));
      //  default 120s
      config.MaxRdSecond = 0;
    }, providerSerializerName).WithMessagePack(providerSerializerName);
});
var settings = builder.Configuration.GetSection("Settings").Get<Settings>();
Console.WriteLine($"Load Settings: ${settings}");

var app = builder.Build();
int count = 0;

app.MapGet("/", () => "Hello World");
app.MapHealthChecks("/healthz");
app.MapGet("/next", async (context) =>
{
  var req = context.Request;
  Console.WriteLine($"Path:{req.Path},Method:${req.Method}");
  count++;
  var source = new Book()
  {
    Id = count.ToString(),
    Secret = Guid.NewGuid().ToString()
  };
  await context.Response.WriteAsJsonAsync(JsonConvert.SerializeObject(source));
});
CancellationTokenSource cancellation = new();
app.MapGet("/delay/{value}", async (int value) =>
{
  try
  {
    await Task.Delay(value, cancellation.Token);
  }
  catch (TaskCanceledException e)
  {
    throw e;
  }
  return new { Delay = value };
});
app.MapGet("/redis/set/{key}", async (IEasyCachingProviderFactory factory, string key) =>
{
  var provider = factory.GetCachingProvider(providerSerializerName);
  // Set Async
  var value = Guid.NewGuid().ToString();
  TimeSpan expire = TimeSpan.FromMinutes(1);
  await provider.SetAsync(key, value, expire);
  return new { Result = "ok", Key = key, Value = value, Expire = expire.TotalSeconds };
});
app.MapGet("/redis/get/{key}", async (IEasyCachingProviderFactory factory, string key) =>
{
  var provider = factory.GetCachingProvider(providerSerializerName);
  // Get Async
  var result = await provider.GetAsync<string>(key);
  if (result == null)
  {
    return new SourceResult()
    {
      Result = "error",
      Value = new Dictionary<string, dynamic>() {
        {"Key", key},
        {"Value", "Not Found"},
        {"Expire", -1},
      }
    };
  }
  var expire = await provider.GetExpirationAsync(key);
  return new SourceResult()
  {
    Result = "ok",
    Value = new Dictionary<string, dynamic>() {
        {"Key", key},
        {"Value", result.Value},
        {"Expire", expire.TotalSeconds},
      }
  };
});
app.MapPost("/post", async (HttpContext context) =>
{
  var req = context.Request;
  Console.WriteLine($"Path:{req.Path},Method:${req.Method}");
  await context.Response.WriteAsJsonAsync(JsonConvert.SerializeObject(new
  {
    method = req.Method,
    date = DateTimeOffset.Now.ToUnixTimeMilliseconds()
  }));
});
Console.WriteLine($"Server running.");
app.Run();
