using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain.Abstract;
using Domain.Messaging;
using MassTransit;
using Service.Abstract;

namespace API.EventConsumers;

public class AnalyzePostMessageConsumer : IConsumer<AnalyzePostMessage>
{
    private readonly ILogger<AnalyzePostMessageConsumer> _logger;
    private readonly IPostRepository _postRepository;
    private readonly ISemanticAnalysisService _semanticAnalysisService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserPunishmentsService _punishmentsService;

    public AnalyzePostMessageConsumer(
        ILogger<AnalyzePostMessageConsumer> logger,
        IPostRepository postRepository,
        ISemanticAnalysisService semanticAnalysisService,
        IUnitOfWork unitOfWork,
        IUserPunishmentsService punishmentsService)
    {
        _logger = logger;
        _postRepository = postRepository;
        _semanticAnalysisService = semanticAnalysisService;
        _unitOfWork = unitOfWork;
        _punishmentsService = punishmentsService;
    }


    public async Task Consume(ConsumeContext<AnalyzePostMessage> context)
    {
        _logger.LogInformation($"Consumed Analyze Post | ${context.Message.PostId}");
        var message = context.Message;
        var ct = context.CancellationToken;
        var post = await _postRepository.GetByIdAsync(message.PostId, ct) 
            ?? throw new NotFoundException($"Post with ID {message.PostId} was not found");

        var result = _semanticAnalysisService.Analyze(post.Content);

        post.DetectedLanguage = result.Language;

        if (!string.IsNullOrEmpty(result.CensoredText))
        {
            post.Content = result.CensoredText;
        }
        await _unitOfWork.CommitAsync(ct);

        if (result.DoPunish)
        {
            await _punishmentsService.Warn(post.UserId, result.PunishmentExplanation, ct);
        }
    }
}