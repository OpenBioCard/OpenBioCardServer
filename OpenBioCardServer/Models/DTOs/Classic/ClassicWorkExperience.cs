using Newtonsoft.Json;

namespace OpenBioCardServer.Models.DTOs.Classic;

public class ClassicWorkExperience
{
    [JsonProperty("position")]
    public string Position { get; set; } = string.Empty;
    
    [JsonProperty("company")]
    public string Company { get; set; } = string.Empty;
    
    [JsonProperty("companyLink")]
    public string CompanyLink { get; set; } = string.Empty;
    
    [JsonProperty("startDate")]
    public string StartDate { get; set; } = string.Empty;
    
    [JsonProperty("endDate")]
    public string EndDate { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("logo")]
    public string Logo { get; set; } = string.Empty;
}