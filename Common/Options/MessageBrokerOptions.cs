namespace Common.Options;

public class MessageBrokerOptions : BaseApplicationOptions
{
    public new static string Config => "MessageBroker";
    public MessageBrokerProvider Provider { get; set; }
}

public enum MessageBrokerProvider
{
    InMemory,
    Rabbit
}