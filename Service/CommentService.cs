using Common.Dto.Paging.CursorPaging;
using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain;
using Service.Abstract;
using System.Linq.Expressions;
using Domain.Abstract;

namespace Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
        }

        public async Task<Comment> Add(Comment entity, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(entity.PostId, cancellationToken)
                ?? throw new ValidationException($"{nameof(Post)} of ID: {entity.PostId} does not exist");

            var comment = await _commentRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return comment;
        }

        public async Task<Comment> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdWithIncludeAsync(id, cancellationToken, e => e.User);

            if (comment == null)
            {
                throw new ValidationException($"{nameof(Comment)} of ID: {id} does not exist");
            }

            return comment;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(int postId, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.GetByPostIdIncludeUserAndPostAsync(postId, cancellationToken);

            return comments;
        }


        public async Task RemoveAsync(int id, int issuerId, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(id, cancellationToken);

            if (comment == null)
            {
                throw new ValidationException($"{nameof(Comment)} of ID: {id} does not exist");
            }

            if (issuerId != comment.UserId)
            {
                throw new InsufficientPermissionsException(
                    $"Authorized {nameof(User)} of ID: {issuerId} has no access to this comment");
            }

            await _commentRepository.RemoveAsync(id, cancellationToken);
        }

        public async Task<Comment> UpdateAsync(Comment entity, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(entity.Id, cancellationToken);

            if (comment == null)
            {
                throw new ValidationException($"{nameof(Comment)} of ID: {entity.Id} does not exist");
            }

            if (entity.UserId != comment.UserId)
            {
                throw new InsufficientPermissionsException("Authorized user has no access to this comment");
            }

            comment.Content = entity.Content;

            return await _commentRepository.Update(comment, cancellationToken);
        }

        public async Task<Comment> GetByIdWithIncludeAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<Comment, object>>[] includeProperties)
        {
            var comment = await _commentRepository.GetByIdWithIncludeAsync(id, cancellationToken, includeProperties);

            return comment ?? throw new ValidationException($"{nameof(Comment)} of ID: {id} does not exist");
        }

        public async Task<CursorPagedResult<Comment>> GetCursorPageAsync(CursorPagedRequest query,
            CancellationToken cancellationToken, params Expression<Func<Comment, object>>[] includeProperties)
        {
            var result = await _commentRepository.GetCursorPagedData(query, cancellationToken, includeProperties);

            return result;
        }
    }
}