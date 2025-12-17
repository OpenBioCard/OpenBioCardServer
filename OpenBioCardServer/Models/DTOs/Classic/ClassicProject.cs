using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicProject
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("logo")]
    public string Logo { get; set; } = string.Empty;
}