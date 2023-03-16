public class MyLoggerOptions
{
  internal IList<IMyLoggerExternsion> Extensions { get; }
  public MyLoggerOptions(ConsoleColor color)
  {
    Extensions = new List<IMyLoggerExternsion>();
  }

  public void RegisterExtension(IMyLoggerExternsion extension)
  {
    Extensions.Add(extension);
  }
}