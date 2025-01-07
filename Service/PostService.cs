using Common.Dto.Paging.CursorPaging;
using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain;
using Service.Abstract;
using System.Linq.Expressions;
using Domain.Abstract;
using Domain.Messaging;
using Service.Abstract.Messaging;

namespace Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _unitOfWork;

        public PostService(
            IPostRepository postRepository,
            IMessageBus bus,
            IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _bus = bus;
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> Add(Post request, CancellationToken cancellationToken)
        {
            if (await _postRepository.GetByTitleAsync(request.Title, cancellationToken) != null)
            {
                throw new ValidationException($"Title {request.Title} is occupied");
            }

            var post = await _postRepository.AddAsync(request, saveChanges: false, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _bus.Publish(new AnalyzePostMessage(post.Id), ct: cancellationToken);

            return post;
        }

        public async Task<Post> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _postRepository.GetByIdAsync(id, cancellationToken);

            if (result == null)
            {
                throw new ValidationException($"{nameof(Post)} of ID: {id} does not exist");
            }

            return result;
        }

        public async Task RemoveAsync(int postId, int issuerId, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(postId, cancellationToken);

            if (post == null)
            {
                throw new ValidationException($"{nameof(Post)} of ID: {postId} does not exist");
            }

            if (post.UserId != issuerId)
            {
                throw new InsufficientPermissionsException($"This {nameof(Post)} does not belong to authorized user");
            }

            await _postRepository.RemoveAsync(postId, cancellationToken);
        }

        public async Task<Post> UpdateAsync(Post request, CancellationToken cancellationToken)
        {
            var postId = request.Id;

            var post = await _postRepository.GetByIdAsync(postId, cancellationToken);

            if (post == null)
            {
                throw new ValidationException($"{nameof(Post)} of {nameof(Post)}Id {postId} was not found");
            }

            if (post.Title != request.Title &&
                await _postRepository.GetByTitleAsync(request.Title, cancellationToken) != null)
            {
                throw new ValidationException($"Title {request.Title} is occupied");
            }

            if (request.UserId != post.UserId)
            {
                throw new InsufficientPermissionsException(
                    $"Authorized user has no privileges to edit this {nameof(Post)} postID:{postId}");
            }

            post.Title = request.Title;
            post.Topic = request.Topic;
            post.Content = request.Content;

            post = await _postRepository.Update(post, cancellationToken);

            await _bus.Publish(new AnalyzePostMessage(post.Id), ct: cancellationToken);

            return post;
        }

        public async Task<Post> GetByIdWithIncludeAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<Post, object>>[] includeProperties)
        {
            var post = await _postRepository.GetByIdWithIncludeAsync(id, cancellationToken, includeProperties);

            return post ?? throw new ValidationException($"{nameof(Post)} of ID: {id} does not exist");
        }

        public async Task<CursorPagedResult<Post>> GetCursorPageAsync(CursorPagedRequest query,
            CancellationToken cancellationToken, params Expression<Func<Post, object>>[] includeProperties)
        {
            var pagedPosts = await _postRepository.GetCursorPagedData(query, cancellationToken, includeProperties);

            return pagedPosts;
        }
    }
}