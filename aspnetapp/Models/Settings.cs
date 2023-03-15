using Newtonsoft.Json;
public class Settings
{
  public string Url { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public override string ToString()
  {
    return JsonConvert.SerializeObject(this);
  }
}