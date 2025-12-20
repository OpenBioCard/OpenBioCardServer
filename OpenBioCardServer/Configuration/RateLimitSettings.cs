using OpenBioCardServer.Structs;

namespace OpenBioCardServer.Configuration;

public class RateLimitSettings
{
    public const string SectionName = "RateLimitSettings";

    public List<RateLimitPolicyConfig> Policies { get; set; } = new();
}