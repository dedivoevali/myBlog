namespace Common.Options;

public class AvatarSizingOptions : BaseApplicationOptions
{
    public new static string Config => "AvatarSizing";
    public Sizing Max { get; set; }
    public Sizing Min { get; set; }
    public struct Sizing
    {
        public double Height { get; set; }
        public double Width { get; set; }
    }   
}