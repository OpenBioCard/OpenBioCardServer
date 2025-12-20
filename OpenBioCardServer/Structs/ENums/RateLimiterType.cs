namespace OpenBioCardServer.Structs.ENums;

public enum RateLimiterType
{
    FixedWindow,
    SlidingWindow,
    TokenBucket,
    Concurrency
}