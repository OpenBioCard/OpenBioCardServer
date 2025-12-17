using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicSignInRequest
{
    [JsonProperty("username")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
}