namespace OpenBioCardServer.Constants;

public class CacheKeys
{
    public const string ClassicPublicSettingsCacheKey = "Classic:System:Settings:Public";
    
    public static string GetProfileCacheKey(string username)
    {
        return $"Profile:{username.Trim().ToLowerInvariant()}";
    }

    public static string GetClassicProfileCacheKey(string username)
    {
        return $"Classic:Profile:{username.Trim().ToLowerInvariant()}";
    }
    
}