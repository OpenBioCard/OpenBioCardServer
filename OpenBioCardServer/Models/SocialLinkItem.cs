namespace OpenBioCardServer.Models;

public class SocialLinkItem
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    
    // 所有扩展数据存储在此字典中，如 `GithubData`
    public Dictionary<string, string> Attributes { get; set; } = new();
}