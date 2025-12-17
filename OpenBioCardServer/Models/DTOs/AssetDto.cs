using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models.DTOs;

public class AssetDto
{
    public AssetType Type { get; set; }
    public string? Text { get; set; }
    public string? DataBase64 { get; set; }
}