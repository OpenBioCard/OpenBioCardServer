using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicContact
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;
}