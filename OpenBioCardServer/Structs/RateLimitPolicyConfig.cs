using OpenBioCardServer.Structs.ENums;

namespace OpenBioCardServer.Structs;

public class RateLimitPolicyConfig
{
    public string PolicyName { get; set; } = string.Empty;
    public RateLimiterType Type { get; set; } = RateLimiterType.FixedWindow;
    
    // 通用参数
    public int PermitLimit { get; set; }
    public int WindowSeconds { get; set; }
    public int QueueLimit { get; set; } = 0;

    // Sliding Window 特有参数
    public int SegmentsPerWindow { get; set; } = 8;
    
    // Token Bucket 特有参数
    public int TokensPerPeriod { get; set; }
    public int ReplenishmentPeriodSeconds { get; set; }
    
    // Concurrency 特有参数
    // (PermitLimit 复用为并发数)
}