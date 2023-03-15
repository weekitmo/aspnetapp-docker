using Newtonsoft.Json;

namespace Aspnetapp.Models
{
  public class Book
  {
    [JsonProperty(PropertyName = "_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public required string Id { get; set; }
    [JsonProperty(PropertyName = "secret")]
    public required string Secret { get; set; }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }
  }
}