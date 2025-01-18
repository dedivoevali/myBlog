using API.Controllers.Base;
using API.Filters;
using AutoMapper;
using Common.Dto.Comment;
using Common.Dto.Paging.CursorPaging;
using Common.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Abstract;

namespace API.Controllers
{
    [Route("api/comments")]
    public class CommentsController : AppBaseController
    {
        private readonly ICommentService _commentsService;
        private readonly IMapper _mapper;
        
        public CommentsController(ICommentService commentsService, IMapper mapper)
        {
            _commentsService = commentsService;
            _mapper = mapper;
        }


        [AllowAnonymous]
        [HttpGet("{commentId:int:min(0)}")]
        public async Task<CommentModel> GetByIdAsync(int commentId, CancellationToken cancellationToken)
        {
            var result =
                await _commentsService.GetByIdWithIncludeAsync(commentId, cancellationToken, e => e.User, e => e.Post);

            var commentModel = _mapper.Map<CommentModel>(result);

            return commentModel;
        }


        [AllowAnonymous]
        [HttpGet("[action]/{postId:int:min(0)}")]
        public async Task<IEnumerable<CommentModel>> GetByPostIdAsync(int postId, CancellationToken cancellationToken)
        {
            var comments = await _commentsService.GetCommentsByPostId(postId, cancellationToken);

            return comments.Select(e => _mapper.Map<CommentModel>(e)).ToList();
        }


        [HttpPost]
        [UpdatesUserActivity]
        public async Task<CommentModel> CreateCommentAsync([FromBody] CommentDto request,
            CancellationToken cancellationToken)
        {
            var commentEntity = _mapper.Map<Comment>(request);
            commentEntity.UserId = CurrentUserId;

            var newlyCreatedComment = await _commentsService.Add(commentEntity, cancellationToken);
            newlyCreatedComment = await _commentsService.GetByIdWithIncludeAsync(newlyCreatedComment.Id, cancellationToken,
                e => e.User,
                e => e.Post);

            return _mapper.Map<CommentModel>(newlyCreatedComment);
        }


        [HttpPut("{commentId:int:min(0)}")]
        [UpdatesUserActivity]
        public async Task<CommentModel> EditCommentAsync(int commentId, [FromBody] CommentDto updateRequest,
            CancellationToken cancellationToken)
        {
            var commentEntity = _mapper.Map<Comment>(updateRequest);

            commentEntity.Id = commentId;
            commentEntity.UserId = CurrentUserId;

            var updatedComment = await _commentsService.UpdateAsync(commentEntity, cancellationToken);

            return _mapper.Map<CommentModel>(updatedComment);
        }

        [HttpDelete("{commentId:int:min(0)}")]
        [UpdatesUserActivity]
        public async Task DeleteCommentAsync(int commentId, CancellationToken cancellationToken)
        {
            await _commentsService.RemoveAsync(commentId, CurrentUserId, cancellationToken);
        }

        [AllowAnonymous]
        [HttpPost("paginated-search-cursor")]
        public async Task<CursorPagedResult<CommentModel>> GetPageWithUserAsync([FromBody] CursorPagedRequest query,
            CancellationToken cancellationToken)
        {
            var result = await _commentsService.GetCursorPageAsync(query, cancellationToken, e => e.User, e => e.Post);


            return new CursorPagedResult<CommentModel>
            {
                PageSize = result.PageSize,
                Total = result.Total,
                Items = result.Items.Select(e => _mapper.Map<CommentModel>(e)).ToList(),
                HeadElementId = result.HeadElementId,
                TailElementId = result.TailElementId
            };
        }
    }
}