namespace Microsoft.Extensions.DependencyInjection
{
  using System;

  public static class MyLoggerServiceExtensions
  {
    public static IServiceCollection AddMyLogger(this IServiceCollection services, Action<MyLoggerOptions> setupAction)
    {
      // default options
      var options = new MyLoggerOptions(ConsoleColor.White);
      setupAction(options);
      foreach (var serviceExtension in options.Extensions)
      {
        serviceExtension.AddServices(services);
      }
      services.AddSingleton(options);

      return services;
    }
  }
}