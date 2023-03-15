using Newtonsoft.Json;
public class SourceResult
{
  public string Result { get; set; } = string.Empty;
  public Dictionary<string, object>? Value { get; set; }
  public override string ToString()
  {
    return JsonConvert.SerializeObject(this);
  }
}