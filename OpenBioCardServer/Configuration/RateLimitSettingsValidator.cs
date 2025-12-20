using Microsoft.Extensions.Options;
using OpenBioCardServer.Structs.ENums;

namespace OpenBioCardServer.Configuration;

public class RateLimitSettingsValidator : IValidateOptions<RateLimitSettings>
{
    public ValidateOptionsResult Validate(string? name, RateLimitSettings options)
    {
        if (options.Policies == null || !options.Policies.Any())
        {
            return ValidateOptionsResult.Success; 
        }

        var failures = new List<string>();

        foreach (var policy in options.Policies)
        {
            if (string.IsNullOrWhiteSpace(policy.PolicyName))
                failures.Add($"Rate Limit Policy must have a name.");

            if (policy.PermitLimit <= 0)
                failures.Add($"Policy '{policy.PolicyName}': PermitLimit must be greater than 0.");

            switch (policy.Type)
            {
                case RateLimiterType.FixedWindow:
                case RateLimiterType.SlidingWindow:
                    if (policy.WindowSeconds <= 0)
                        failures.Add($"Policy '{policy.PolicyName}': WindowSeconds must be greater than 0.");
                    break;
                case RateLimiterType.TokenBucket:
                    if (policy.ReplenishmentPeriodSeconds <= 0)
                        failures.Add($"Policy '{policy.PolicyName}': ReplenishmentPeriodSeconds must be greater than 0.");
                    break;
            }
        }


        // 检查重名
        if (options.Policies.Select(p => p.PolicyName).Distinct().Count() != options.Policies.Count)
        {
            failures.Add("Rate Limit Policy names must be unique.");
        }

        return failures.Count > 0 
            ? ValidateOptionsResult.Fail(failures) 
            : ValidateOptionsResult.Success;
    }
}