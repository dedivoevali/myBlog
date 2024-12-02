using API.Controllers.Base;
using API.Filters;
using AutoMapper;
using Common.Dto.PostReaction;
using Common.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Abstract;

namespace API.Controllers
{
    [Route("api/reactions")]
    public class PostReactionController : AppBaseController
    {
        private readonly IPostReactionService _postReactionService;
        private readonly IMapper _mapper;

        public PostReactionController(IPostReactionService postReactionService, IMapper mapper)
        {
            _postReactionService = postReactionService;
            _mapper = mapper;
        }


        [AllowAnonymous]
        [HttpGet("{postId:int}")]
        public async Task<IEnumerable<PostReactionModel>> GetByPostIdAsync(int postId,
            CancellationToken cancellationToken)
        {
            var result = await _postReactionService.GetByPostId(postId, cancellationToken);

            return result.Select(e => _mapper.Map<PostReactionModel>(e));
        }


        [HttpPost]
        [UpdatesUserActivity]
        public async Task<PostReactionModel> CreateReactionAsync([FromBody] PostReactionDto dto,
            CancellationToken cancellationToken)
        {
            var request = _mapper.Map<PostReaction>(dto);
            request.UserId = CurrentUserId;

            var createdPost = await _postReactionService.Add(request, cancellationToken);

            var newlyCreatedReaction = _mapper.Map<PostReactionModel>(createdPost);

            return newlyCreatedReaction;
        }

        [HttpPut]
        [UpdatesUserActivity]
        public async Task<PostReactionModel> EditReactionAsync([FromBody] PostReactionDto dto,
            CancellationToken cancellationToken)
        {
            var request = _mapper.Map<PostReaction>(dto);
            request.UserId = CurrentUserId;

            var editedReaction = await _postReactionService.UpdateAsync(request, cancellationToken);

            return _mapper.Map<PostReactionModel>(editedReaction);
        }

        [HttpDelete("{postId:int}")]
        [UpdatesUserActivity]
        public async Task RemoveReactionByPostIdAsync(int postId, CancellationToken cancellationToken)
        {
            await _postReactionService.RemoveByPostId(CurrentUserId, postId, cancellationToken);
        }
    }
}