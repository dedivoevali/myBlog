namespace Common.Options;

public class CacheOptions : BaseApplicationOptions
{
    public new static string Config => "CacheOptions"; 
    public int DefaultExpirationInMinutes { get; init; }
    public CacheProvider Provider { get; init; } = CacheProvider.InMemory;
}

public enum CacheProvider
{
    InMemory = 0,
    Redis = 1
}