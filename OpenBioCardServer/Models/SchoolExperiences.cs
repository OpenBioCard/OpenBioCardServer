namespace OpenBioCardServer.Models;

/// <summary>
/// 教育经历
/// </summary>
public class SchoolExperiences
{
    public string School { get; set; } = string.Empty;
    public string? SchoolLink { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string? Major { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Description { get; set; }
    
    /// <summary>
    /// 学校Logo - 必须是图片格式
    /// 存储格式：BASE64 图片（前端提交）或 "asset:{GUID}"（后端存储）
    /// </summary>
    public string? Logo { get; set; }
    
    public bool IsCurrent { get; set; }
}