using Microsoft.Extensions.Options;

namespace OpenBioCardServer.Configuration;

public class AssetSettingsValidator : IValidateOptions<AssetSettings>
{
    public ValidateOptionsResult Validate(string? name, AssetSettings options)
    {
        if (options.MaxFileSizeBytes <= 0)
        {
            return ValidateOptionsResult.Fail(
                "AssetSettings.MaxFileSizeBytes must be greater than 0");
        }

        if (options.MaxFileSizeBytes > 50 * 1024 * 1024) // 50MB
        {
            return ValidateOptionsResult.Fail(
                "AssetSettings.MaxFileSizeBytes cannot exceed 50MB");
        }

        if (options.AllowedImageTypes == null || !options.AllowedImageTypes.Any())
        {
            return ValidateOptionsResult.Fail(
                "AssetSettings.AllowedImageTypes must contain at least one image type");
        }

        return ValidateOptionsResult.Success;
    }
}