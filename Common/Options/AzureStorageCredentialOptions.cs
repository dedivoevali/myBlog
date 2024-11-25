using Azure.Storage;

namespace Common.Options;

public class AzureStorageCredentialOptions : BaseApplicationOptions
{
    public new static string Config => "AzureStorage";
    public string ConnectionString { get; set; }
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
    public StorageSharedKeyCredential BuildSasKey() => new(AccountName, AccountKey);
}