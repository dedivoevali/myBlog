using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain;
using Service.Abstract;
using System.Linq.Expressions;
using Domain.Abstract;

namespace Service
{
    public class PostReactionService : IPostReactionService
    {
        private readonly IPostReactionRepository _postReactionRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostReactionService(IPostReactionRepository postReactionRepository, IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postReactionRepository = postReactionRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PostReaction> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionRepository.GetByIdAsync(id, cancellationToken);

            if (reaction == null)
            {
                throw new ValidationException($"{nameof(Comment)} of ID: {id} does not exist");
            }

            return reaction;
        }

        public async Task<IEnumerable<PostReaction>> GetByPostId(int postId, CancellationToken cancellationToken)
        {
            if (await PostDoesNotExistAsync(postId, cancellationToken))
            {
                throw new ValidationException($"{nameof(Post)} of ID: {postId} does not exist");
            }

            return await _postReactionRepository.GetWhereAsync(p => p.PostId == postId, cancellationToken);
        }

        public async Task<PostReaction> Add(PostReaction entity, CancellationToken cancellationToken)
        {
            if (await PostDoesNotExistAsync(entity.PostId, cancellationToken))
            {
                throw new ValidationException($"{nameof(Post)} of ID: {entity.PostId} does not exist");
            }

            if (await ExistsReactionOfUserAsync(entity.PostId, entity.UserId, cancellationToken))
            {
                throw new ValidationException(
                    $"{nameof(Post)}ID: {entity.PostId} already has found reaction from {nameof(User)}ID: {entity.UserId}");
            }

            var reaction = await _postReactionRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return reaction;
        }

        public async Task RemoveAsync(int reactionId, int issuerId, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionRepository.GetByIdAsync(reactionId, cancellationToken);

            if (reaction == null)
            {
                throw new ValidationException($"Object ID : {reactionId} of {nameof(PostReaction)} does not exist");
            }

            if (reaction.UserId != issuerId)
            {
                throw new InsufficientPermissionsException(
                    $"This {nameof(PostReaction)} does not belong to authorized user");
            }

            await _postReactionRepository.RemoveAsync(reactionId, cancellationToken);
        }

        public async Task<PostReaction> UpdateAsync(PostReaction entity, CancellationToken cancellationToken)
        {
            if (!await ExistsReactionOfUserAsync(entity.PostId, entity.UserId, cancellationToken))
            {
                throw new ValidationException(
                    $"{nameof(Post)}ID: {entity.PostId} has no reaction from {nameof(User)}ID:{entity.UserId}");
            }

            var found = await _postReactionRepository.GetWhereAsync(
                r => r.PostId == entity.PostId && r.UserId == entity.UserId, cancellationToken);

            var existingReaction = found.FirstOrDefault() ?? throw new ValidationException("Reaction was not found");


            existingReaction.ReactionType = entity.ReactionType;

            return await _postReactionRepository.Update(existingReaction, cancellationToken);
        }

        public async Task<PostReaction> GetByIdWithIncludeAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<PostReaction, object>>[] includeProperties)
        {
            var reaction =
                await _postReactionRepository.GetByIdWithIncludeAsync(id, cancellationToken, includeProperties);

            return reaction ?? throw new ValidationException($"{nameof(Comment)} of ID: {id} does not exist");
        }

        public async Task RemoveByPostId(int issuerId, int postId, CancellationToken cancellationToken)
        {
            if (await PostDoesNotExistAsync(postId, cancellationToken))
            {
                throw new ValidationException($"{nameof(Post)} of ID: {postId} does not exist");
            }

            if (!await ExistsReactionOfUserAsync(postId, issuerId, cancellationToken))
            {
                throw new ValidationException(
                    $"{nameof(Post)}ID: {postId} has no reaction from {nameof(User)}ID:{issuerId}");
            }

            var matchingReactions =
                await _postReactionRepository.GetWhereAsync(e => e.PostId == postId && e.UserId == issuerId,
                    cancellationToken);

            var postReaction = matchingReactions.FirstOrDefault() ?? throw new ValidationException("Reaction was not found");

            await _postReactionRepository.RemoveAsync(postReaction.Id, cancellationToken);
        }

        private async Task<bool> ExistsReactionOfUserAsync(int postId, int userId, CancellationToken cancellationToken)
        {
            var requestedPostReactionsPostedByUser =
                await _postReactionRepository.GetWhereAsync(r => r.PostId == postId && r.UserId == userId,
                    cancellationToken);

            return requestedPostReactionsPostedByUser.Any();
        }

        private async Task<bool> PostDoesNotExistAsync(int postId, CancellationToken cancellationToken)
        {
            var searchRequestForPost = await _postRepository.GetByIdAsync(postId, cancellationToken);

            return searchRequestForPost == null;
        }
    }
}