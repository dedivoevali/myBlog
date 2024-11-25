using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Common.Options;
using Microsoft.Extensions.Options;
using Service.Abstract;

namespace Service;

public class AzureBlobStorageService<TContainer> : IBlobStorageService<TContainer> where TContainer : BlobContainerOptions
{
    private readonly AzureStorageCredentialOptions _credentials;
    private readonly TContainer _container;
    public AzureBlobStorageService(
        IOptions<AzureStorageCredentialOptions> credentialOptions,
        IOptions<TContainer> containerOptions)
    {
        _credentials = credentialOptions.Value;
        _container = containerOptions.Value;
    }

    private async Task<BlobClient> GetClient(string blobName, CancellationToken ct)
    {
        var client = new BlobContainerClient(_credentials.ConnectionString, _container.Name);
        await client.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: ct);
        return client.GetBlobClient(blobName);
    }
    public async Task UploadBlob(string name, byte[] data, string contentType, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(contentType);
        ArgumentNullException.ThrowIfNull(data);

        var client = await GetClient(name, ct);
        await using var stream = new MemoryStream(data);
        await client
            .UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
    }

    public async Task<byte[]> GetBlob(string name, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var client = await GetClient(name, ct);
        await using var stream = new MemoryStream();
        await client.DownloadToAsync(stream, cancellationToken: ct);
        return stream.ToArray();
    }

    public async Task DeleteBlob(string name, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var client = await GetClient(name, ct);
        await client.DeleteIfExistsAsync(cancellationToken: ct);
    }

    public async Task<string> GetBlobUrl(string name, DateTime? expiresAfterUtc = null, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        expiresAfterUtc ??= DateTime.UtcNow.AddMinutes(5);

        var client = await GetClient(name, ct);

        var sasBuilder = new BlobSasBuilder(BlobContainerSasPermissions.Read, new DateTimeOffset(expiresAfterUtc.Value))
        {
            BlobContainerName = client.BlobContainerName,
            BlobName = client.Name,
            Resource = "b"
        };
        
        var key = _credentials.BuildSasKey();
        var blobUriBuilder = new BlobUriBuilder(client.Uri)
        {
            Sas = sasBuilder.ToSasQueryParameters(key)
        };
        return blobUriBuilder.ToUri().ToString();
    }
}