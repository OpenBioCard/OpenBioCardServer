namespace OpenBioCardServer.Utilities;

public static class Base64Utility
{
    /// <summary>
    /// Creates an image data URL from binary data using zero-copy Base64 encoding
    /// </summary>
    /// <param name="data">Binary image data</param>
    /// <param name="mimeType">MIME type (e.g., "image/png")</param>
    /// <returns>Data URL string in format "data:image/png;base64,..."</returns>
    public static string CreateImageDataUrl(byte[] data, string mimeType)
    {
        var base64Length = GetBase64EncodedLength(data.Length);
        var totalLength = 5 + mimeType.Length + 8 + base64Length; // "data:" + mimeType + ";base64," + base64

        return string.Create(totalLength, (data, mimeType), static (span, state) =>
        {
            var current = span;

            "data:".AsSpan().CopyTo(current);
            current = current[5..];

            state.mimeType.AsSpan().CopyTo(current);
            current = current[state.mimeType.Length..];

            ";base64,".AsSpan().CopyTo(current);
            current = current[8..];

            Convert.TryToBase64Chars(state.data, current, out _);
        });
    }

    /// <summary>
    /// Calculates the length of Base64 encoded string from binary data length
    /// </summary>
    /// <param name="byteLength">Length of binary data in bytes</param>
    /// <returns>Length of Base64 encoded string</returns>
    public static int GetBase64EncodedLength(int byteLength) => ((byteLength + 2) / 3) * 4;

    /// <summary>
    /// Attempts to parse a Base64 image data URL and extract the binary data
    /// </summary>
    /// <param name="dataUrl">Data URL string starting with "data:image/"</param>
    /// <param name="data">Decoded binary data if successful</param>
    /// <returns>True if parsing succeeded, false otherwise</returns>
    public static bool TryParseImageDataUrl(string dataUrl, out byte[] data)
    {
        data = Array.Empty<byte>();

        if (string.IsNullOrEmpty(dataUrl) || !dataUrl.StartsWith("data:image/"))
            return false;

        var parts = dataUrl.Split(',', 2);
        if (parts.Length != 2)
            return false;

        try
        {
            data = Convert.FromBase64String(parts[1]);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extracts the MIME type from a data URL
    /// </summary>
    /// <param name="dataUrl">Data URL string</param>
    /// <returns>MIME type (e.g., "image/png") or null if invalid</returns>
    public static string? GetMimeTypeFromDataUrl(string dataUrl)
    {
        if (string.IsNullOrEmpty(dataUrl) || !dataUrl.StartsWith("data:"))
            return null;

        var semicolonIndex = dataUrl.IndexOf(';');
        if (semicolonIndex < 0)
            return null;

        return dataUrl[5..semicolonIndex]; // Skip "data:"
    }

    /// <summary>
    /// Checks if a string is a valid Base64 data URL
    /// </summary>
    /// <param name="value">String to check</param>
    /// <returns>True if the string is a Base64 data URL</returns>
    public static bool IsBase64DataUrl(string? value) =>
        !string.IsNullOrEmpty(value) && value.StartsWith("data:") && value.Contains(";base64,");
}
