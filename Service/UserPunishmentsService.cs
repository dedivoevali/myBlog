using Common.Exceptions;
using Common.Options;
using DAL.Repositories.Abstract;
using Domain;
using Domain.Abstract;
using Microsoft.Extensions.Options;
using Service.Abstract;

namespace Service;

public class UserPunishmentsService : IUserPunishmentsService
{
    private readonly IUserBanLogRepository _banLogRepository;
    private readonly IUserWarningRepository _warningRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PunishmentOptions _punishmentOptions;

    public UserPunishmentsService(
        IUserBanLogRepository banLogRepository,
        IUserWarningRepository warningRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IOptions<PunishmentOptions> punishmentOptions)
    {
        _punishmentOptions = punishmentOptions.Value;
        _banLogRepository = banLogRepository;
        _warningRepository = warningRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Ban(int userId, string reason, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException($"User with ID {userId} was not found");

        if (user.IsBanned)
        {
            throw new ValidationException($"User (ID: {userId}) is already banned!");
        }

        var ban = new UserBanLog
        {
            Reason = reason,
            UserId = userId,
            Action = BanAction.Ban
        };

        await _banLogRepository.AddAsync(ban, ct);
        user.IsBanned = true;
        await _unitOfWork.CommitAsync(ct);
    }

    public async Task Unban(int userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException($"User with ID {userId} was not found");
        
        if (!user.IsBanned)
        {
            throw new ValidationException($"User (ID: {userId}) isn't banned!");
        }

        var ban = new UserBanLog
        {
            UserId = userId,
            Action = BanAction.Unban
        };
        await _banLogRepository.AddAsync(ban, ct);
        user.IsBanned = false;
        await _unitOfWork.CommitAsync(ct);
    }

    public async Task Warn(int userId, string reason, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        var user = await _userRepository.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException($"User with ID {userId} was not found");

        var newWarn = new UserWarning
        {
            UserId = user.Id,
            Reason = reason
        };

        await _warningRepository.AddAsync(newWarn, ct);
        await _unitOfWork.CommitAsync(ct);

        var maximumAllowedWarnings = _punishmentOptions.MaxAllowedWarns;
        var currentWarnings = await _warningRepository.GetUserActiveWarningsCount(userId, ct);
        if (currentWarnings >= maximumAllowedWarnings)
        {
            await Ban(userId, $"Warnings limit exceeded {maximumAllowedWarnings}", ct);
        }
    }

    public async Task Unwarn(int warnId, CancellationToken ct = default)
    {
        var warn = await _warningRepository.GetByIdAsync(warnId, ct)
            ?? throw new NotFoundException($"Warning with ID {warnId} wasn't found");

        warn.RemovedAt = DateTime.UtcNow;
        await _unitOfWork.CommitAsync(ct);
    }
}