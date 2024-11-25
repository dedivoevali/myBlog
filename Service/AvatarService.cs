using Common.Exceptions;
using Common.Extensions;
using DAL.Repositories.Abstract;
using Domain;
using Microsoft.AspNetCore.Http;
using Service.Abstract;
using Common.Options;
using Domain.Abstract;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;

namespace Service
{
    public class AvatarService : IAvatarService
    {
        private readonly IAvatarRepository _avatarRepository;
        private readonly IBlobStorageService<AvatarContainerOptions> _blob;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AvatarSizingOptions _sizingOptions;

        public AvatarService(IAvatarRepository avatarRepository,
            IBlobStorageService<AvatarContainerOptions> blob,
            IOptions<AvatarSizingOptions> sizingOptions,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _avatarRepository = avatarRepository;
            _blob = blob;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _sizingOptions = sizingOptions.Value;
        }

        public async Task<string> AddAndGetUrlAsync(IFormFile image, int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException($"UID: ${userId} not found");

            var imgBytes = FormFileToByteArray(image);
            var img = Image.Load(imgBytes);

            ValidateImageSize(img);

            var fileName = user.Id.ToString();

            await _blob.UploadBlob(fileName, imgBytes, image.ContentType, cancellationToken);

            var entity = new Avatar
            {
                UserId = userId,
                BlobName = fileName
            };

            await _avatarRepository.AddAsync(entity, false, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return await _blob.GetBlobUrl(fileName, ct: cancellationToken);
        }

        public async Task<string> GetAvatarUrlAsync(int userId, CancellationToken cancellationToken)
        {
            if (await _avatarRepository.HasAvatar(userId, cancellationToken))
            {
                return await _blob.GetBlobUrl(userId.ToString(), ct: cancellationToken);
            }

            return string.Empty;
        }

        public async Task RemoveAsync(int userId, CancellationToken cancellationToken)
        {
            var avatar = await _avatarRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException($"UID ${userId} has no avatar");

            await _blob.DeleteBlob(avatar.BlobName, cancellationToken);

            await _avatarRepository.RemoveAsync(avatar.Id, cancellationToken);
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
            if (image.Height > _sizingOptions.Max.Height || image.Height < _sizingOptions.Min.Height)
            {
                throw new ValidationException($"Height should be between ${_sizingOptions.Min.Height} and ${_sizingOptions.Max.Height}");
            }

            if (image.Width > _sizingOptions.Max.Width || image.Width < _sizingOptions.Min.Width)
            {
                throw new ValidationException($"Width should be between ${_sizingOptions.Min.Width} and ${_sizingOptions.Max.Width}");
            }
        }
    }
}