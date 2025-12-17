using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models;

public class Asset
{
    public AssetType Type { get; set; }
    public string? Text { get; set; } // Text & Emoji & Remote Uri
    public byte[]? Data { get; set; } // Embedded Base64
}