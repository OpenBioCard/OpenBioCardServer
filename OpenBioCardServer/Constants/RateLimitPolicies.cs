namespace OpenBioCardServer.Constants;

public class RateLimitPolicies
{
    /// <summary>
    /// Strict limit for authentication endpoints (e.g., 5 req/min)
    /// </summary>
    public const string Login = "login";
    
    /// <summary>
    /// Standard limit for general API usage (e.g., 60 req/min)
    /// </summary>
    public const string General = "general";
}