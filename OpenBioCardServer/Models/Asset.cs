using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models;

public class Asset
{
    public AssetType Type { get; set; }
    public string? Text { get; set; } // Text & Emoji & Remote Uri
    public string? Tag { get; set; } // MIME Type
    public byte[]? Data { get; set; } // Embedded Base64
}