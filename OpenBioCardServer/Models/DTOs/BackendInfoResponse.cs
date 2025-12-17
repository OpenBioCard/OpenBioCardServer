namespace OpenBioCardServer.Models.DTOs;

/// <summary>
/// 后端信息响应
/// </summary>
public class BackendInfoResponse
{
    public string Backend { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string? BuildDate { get; set; }
}