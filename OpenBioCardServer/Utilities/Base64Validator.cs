namespace OpenBioCardServer.Utilities;

public static class Base64Validator
{
    /// <summary>
    /// Calculate the approximate size of decoded Base64 data
    /// </summary>
    public static long CalculateBase64Size(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
            return 0;

        // Remove data URL prefix if present
        var base64Data = base64String;
        if (base64String.StartsWith("data:"))
        {
            var commaIndex = base64String.IndexOf(',');
            if (commaIndex >= 0)
            {
                base64Data = base64String[(commaIndex + 1)..];
            }
        }

        // Remove padding and calculate size
        var paddingCount = base64Data.EndsWith("==") ? 2 : base64Data.EndsWith("=") ? 1 : 0;
        var base64Length = base64Data.Length;
        
        // Decoded size = (base64Length * 3) / 4 - paddingCount
        return (base64Length * 3L / 4) - paddingCount;
    }

    /// <summary>
    /// Validate if Base64 string size is within limit
    /// </summary>
    public static (bool isValid, string? errorMessage) ValidateSize(
        string base64String, 
        long maxSizeBytes,
        string fieldName = "File")
    {
        if (string.IsNullOrEmpty(base64String))
            return (true, null);

        var estimatedSize = CalculateBase64Size(base64String);
        
        if (estimatedSize > maxSizeBytes)
        {
            var maxSizeMB = maxSizeBytes / 1024.0 / 1024.0;
            var actualSizeMB = estimatedSize / 1024.0 / 1024.0;
            
            return (false, 
                $"{fieldName} size ({actualSizeMB:F2}MB) exceeds maximum allowed size ({maxSizeMB:F2}MB)");
        }

        return (true, null);
    }

    /// <summary>
    /// Validate image type from Base64 data URL
    /// </summary>
    public static (bool isValid, string? errorMessage) ValidateImageType(
        string base64String,
        IEnumerable<string> allowedTypes,
        string fieldName = "File")
    {
        if (string.IsNullOrEmpty(base64String))
            return (true, null);

        if (!base64String.StartsWith("data:"))
            return (true, null); // Not a data URL, skip type check

        var semicolonIndex = base64String.IndexOf(';');
        if (semicolonIndex < 0)
            return (false, $"{fieldName} has invalid data URL format");

        var mimeType = base64String[5..semicolonIndex]; // Skip "data:"

        if (!allowedTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
        {
            return (false, 
                $"{fieldName} type '{mimeType}' is not allowed. " +
                $"Allowed types: {string.Join(", ", allowedTypes)}");
        }

        return (true, null);
    }
}
