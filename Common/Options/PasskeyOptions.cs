namespace Common.Options;

public class PasskeyOptions : BaseApplicationOptions
{
    public new static string Config => "PasskeyOptions";

    public PasskeyRelyingParty RelyingParty { get; set; }
    public int ChallengeLifetimeMinutes { get; set; }
    public TimeSpan ChallengeLifetime => TimeSpan.FromMinutes(ChallengeLifetimeMinutes);
}

public class PasskeyRelyingParty
{
    public string DomainName { get; init; }
    public string DisplayName { get; init; }
    public string Icon { get; init; }
    public IEnumerable<string> Origins { get; set; }
}