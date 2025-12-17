using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicSchoolExperience
{
    [JsonProperty("degree")]
    public string Degree { get; set; } = string.Empty;
    
    [JsonProperty("school")]
    public string School { get; set; } = string.Empty;
    
    [JsonProperty("schoolLink")]
    public string SchoolLink { get; set; } = string.Empty;
    
    [JsonProperty("major")]
    public string Major { get; set; } = string.Empty;
    
    [JsonProperty("startDate")]
    public string StartDate { get; set; } = string.Empty;
    
    [JsonProperty("endDate")]
    public string EndDate { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("logo")]
    public string Logo { get; set; } = string.Empty;
}