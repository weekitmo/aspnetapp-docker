using Aspnetapp.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
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
