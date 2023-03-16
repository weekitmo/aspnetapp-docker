namespace Microsoft.Extensions.DependencyInjection
{
  using System;
  using Microsoft.Extensions.Options;

  public static class MyLoggerOptionsExtension
  {
    public static MyLoggerOptions UseInfoLogger(
            this MyLoggerOptions options
            , Action<MyInfoLoggerOptions> configure
            , string name
            )
    {
      options.RegisterExtension(new MyInfoLoggerOptionsExtension(name, configure));
      return options;
    }
  }

  internal sealed class MyInfoLoggerOptionsExtension : IMyLoggerExternsion
  {
    private readonly string _name;
    private readonly Action<MyInfoLoggerOptions> configure;

    public MyInfoLoggerOptionsExtension(string name, Action<MyInfoLoggerOptions> configure)
    {
      this._name = name;
      this.configure = configure;
    }

    public void AddServices(IServiceCollection services)
    {
      services.Configure(_name, configure);
      services.AddSingleton<ILog, MyInfoLogger>(x =>
      {
        var optionsMon = x.GetRequiredService<IOptionsMonitor<MyInfoLoggerOptions>>();
        var options = optionsMon.Get(_name);
        return new MyInfoLogger(options);
      });
    }

    public class MyInfoLogger : ILog
    {
      private MyInfoLoggerOptions? _options;
      public MyInfoLogger(MyInfoLoggerOptions? options)
      {
        this._options = options;
      }
      public void info(string str)
      {
        if (_options != null)
        {
          Console.ForegroundColor = _options.ForegroundColor;
          if (_options.BackgroundColor != null)
          {
            Console.BackgroundColor = (ConsoleColor)_options.BackgroundColor;
          }
        }

        Console.WriteLine(str);
        Console.ResetColor();
      }
    }
  }
}