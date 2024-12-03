using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Options;

public class AvatarOptions : BaseApplicationOptions
{
    public new static string Config => "Avatar";
    public Sizing Max { get; set; }
    public Sizing Min { get; set; }
    public struct Sizing
    {
        public double Height { get; set; }
        public double Width { get; set; }
    }   
    public int CacheRetentionMinutes { get; set; }
    [NotMapped]
    public TimeSpan CacheRetention => TimeSpan.FromMinutes(CacheRetentionMinutes);
}