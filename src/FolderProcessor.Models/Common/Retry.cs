using JetBrains.Annotations;

namespace FolderProcessor.Models.Common;

/// <summary>
/// A simple struct for telling algorithms to retry their operations on failure.
/// </summary>
[PublicAPI]
public readonly struct Retry
{
    /// <summary>
    /// Creates a new retry type that instructs algorithms not to retry
    /// </summary>
    public static readonly Retry None = new(RetryType.None, 0);
    public static Retry Fixed(TimeSpan interval, int retryCount = 5)
    {
        return new Retry(interval);
    }

    public static Retry Exponential(int retryCount = 5)
    {
        return new Retry(RetryType.Exponential, retryCount);
    }

    /// <summary>
    /// The maximum number of times the algorithm should attempt to retry an
    /// action before throwing an exception. If this number is less than zero,
    /// the algorithm will keep trying until cancelled. 
    /// </summary>
    public int RetryCount { get; }

    /// <summary>
    /// The amount of time to wait before attempting to rerun the algorithm,
    /// this value will only be set for fixed retries. 
    /// </summary>
    public TimeSpan WaitTime { get; }

    /// <summary>
    /// Denotes if the consuming algorithm should retry or not.
    /// </summary>
    public bool ShouldRetry => RetryCount != 0;
    
    /// <summary>
    /// Instructs the consuming algorithm to used a fixed timeout mechanism 
    /// </summary>
    public bool IsFixed => _retryType == RetryType.Fixed;
    
    /// <summary>
    /// Instructs the consuming algorithm not to use retry.
    /// </summary>
    public bool IsNone => _retryType == RetryType.None;
    
    /// <summary>
    /// Instructs the consuming algorithm to use an exponential timeout mechanism
    /// </summary>
    public bool IsExponential => _retryType == RetryType.Exponential;
    
    private readonly RetryType _retryType;
    
    private Retry(
        RetryType type, 
        int retryCount = 5)
    {
        _retryType = type;
        WaitTime = TimeSpan.Zero;
        RetryCount = retryCount;
    }

    private Retry(TimeSpan interval, int retryCount = 5) : 
        this(RetryType.Fixed, retryCount)
    {
        WaitTime = interval;
    }
}

public enum RetryType
{
    None,
    Fixed,
    Exponential
}
