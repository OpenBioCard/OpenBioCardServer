using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicAdminRequest
{
    [JsonProperty("username")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;
}