namespace OpenBioCardServer.Models;

/// <summary>
/// 工作经历
/// </summary>
public class WorkExperience
{
    public string Position { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? CompanyLink { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Description { get; set; }
    
    /// <summary>
    /// 公司Logo - 必须是图片格式
    /// 存储格式：BASE64 图片（前端提交）或 "asset:{GUID}"（后端存储）
    /// </summary>
    public string? Logo { get; set; }
    
    public bool IsCurrent { get; set; }
}