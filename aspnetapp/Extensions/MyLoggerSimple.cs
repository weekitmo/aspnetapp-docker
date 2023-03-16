public interface ILog
{
  void info(string str);
}

// simple use
class MyLogger : ILog
{
  public void info(string str)
  {

    Console.WriteLine(str);
  }
}