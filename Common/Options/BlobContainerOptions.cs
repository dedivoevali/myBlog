namespace Common.Options;

public abstract class BlobContainerOptions
{
    public string Name { get; init; }
}

public class AvatarContainerOptions : BlobContainerOptions;