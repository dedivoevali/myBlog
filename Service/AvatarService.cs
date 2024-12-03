using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain;
using Microsoft.AspNetCore.Http;
using Service.Abstract;
using Common.Options;
using Domain.Abstract;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using Microsoft.Extensions.Logging;

namespace Service
{
    /// <summary>
    /// Blobs are saved under the userid, f.e user with ID "4" uploads his avatar and it'll be saved as blob
    /// with name "4".
    /// </summary>
    public class AvatarService : IAvatarService
    {
        private readonly IAvatarRepository _avatarRepository;
        private readonly IBlobStorageService<AvatarContainerOptions> _blob;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        private readonly AvatarOptions _options;
        private readonly ILogger<AvatarService> _logger;

        public AvatarService(IAvatarRepository avatarRepository,
            IBlobStorageService<AvatarContainerOptions> blob,
            IOptions<AvatarOptions> sizingOptions,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICacheService cache,
            ILogger<AvatarService> logger)
        {
            _avatarRepository = avatarRepository;
            _blob = blob;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _options = sizingOptions.Value;
            _cache = cache;
            _logger = logger;
        }

        public static string GetAvatarUrlCacheKey(int userId) => $"avatar-url-{userId}";

        public async Task<string?> AddAndGetUrlAsync(IFormFile image, int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException($"UID: ${userId} not found");

            var imgBytes = FormFileToByteArray(image);
            var img = Image.Load(imgBytes);

            ValidateImageSize(img);

            var fileName = user.Id.ToString();

            await _blob.UploadBlob(fileName, imgBytes, image.ContentType, cancellationToken);

            var existingAvatar = await _avatarRepository.GetByUserIdAsync(user.Id, cancellationToken);

            if (existingAvatar == null)
            {
                var entity = new Avatar
                {
                    UserId = userId,
                    BlobName = fileName
                };
                await _avatarRepository.AddAsync(entity, false, cancellationToken);
            }
            else
            {
                existingAvatar.BlobName = fileName;
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            await PurgeCache(userId, cancellationToken);
            return await GetBlobUrlWithCache(user.Id, cancellationToken);
        }

        public async Task<string?> GetAvatarUrlAsync(int userId, CancellationToken cancellationToken)
        {
            if (await _avatarRepository.HasAvatar(userId, cancellationToken))
            {
                return await GetBlobUrlWithCache(userId, cancellationToken);
            }

            return string.Empty;
        }

        public async Task RemoveAsync(int userId, CancellationToken cancellationToken)
        {
            var avatar = await _avatarRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException($"UID ${userId} has no avatar");

            await _blob.DeleteBlob(avatar.BlobName, cancellationToken);

            await _avatarRepository.RemoveAsync(avatar.Id, cancellationToken);
            await PurgeCache(userId, cancellationToken);
        }

        private static byte[] FormFileToByteArray(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var bytes = ms.ToArray();
                return bytes;
            }
        }

        private void ValidateImageSize(Image image)
        {
            if (image.Height > _options.Max.Height || image.Height < _options.Min.Height)
            {
                throw new ValidationException($"Height should be between ${_options.Min.Height} and ${_options.Max.Height}");
            }

            if (image.Width > _options.Max.Width || image.Width < _options.Min.Width)
            {
                throw new ValidationException($"Width should be between ${_options.Min.Width} and ${_options.Max.Width}");
            }
        }

        private async Task<string?> GetBlobUrlWithCache(int userId, CancellationToken ct)
        {
            var blobName = userId.ToString();
            var cacheKey = GetAvatarUrlCacheKey(userId);
            var cachedUrl = await _cache.GetStringAsync(cacheKey, ct);

            if (cachedUrl != null)
            {
                return cachedUrl;
            }

            var expiresAt = DateTime.UtcNow.Add(_options.CacheRetention);
            var url = await _blob.GetBlobUrl(blobName, expiresAt, ct);
            if (url != null)
            {
                await _cache.SetAsync(cacheKey, url, _options.CacheRetention, ct);
            }
            else
            {
                _logger.LogError("UID: {0} has avatar in database, but blob was not found", userId);
            }
            return url;
        }

        private async Task PurgeCache(int userId, CancellationToken ct)
        {
            var cacheKey = GetAvatarUrlCacheKey(userId);
            await _cache.RemoveAsync(cacheKey, ct);
        }
    }
}