using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicSocialLink
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonProperty("githubData")]
    public Dictionary<string, object>? GithubData { get; set; }
}