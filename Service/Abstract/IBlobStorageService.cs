using Common.Options;

namespace Service.Abstract;
public interface IBlobStorageService<TContainer> where TContainer : BlobContainerOptions
{
    Task UploadBlob(string name, byte[] data, string contentType, CancellationToken ct = default);
    Task<byte[]> GetBlob(string name, CancellationToken ct = default);
    Task DeleteBlob(string name, CancellationToken ct = default);
    /// <param name="expiresAfterUtc">If not specified, would take default expiration timeout</param>
    Task<string> GetBlobUrl(string name, DateTime? expiresAfterUtc = null, CancellationToken ct = default);
}
