using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicGalleryItem
{
    [JsonProperty("image")]
    public string Image { get; set; } = string.Empty;
    
    [JsonProperty("caption")]
    public string Caption { get; set; } = string.Empty;
}