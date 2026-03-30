using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;

namespace BigDaddy.Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IUnitOfWork _uow;

    public ResetPasswordCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(ResetPasswordCommand command, CancellationToken ct = default)
    {
        if (command.NewPassword != command.ConfirmPassword)
            throw new ConflictException("New password and confirmation do not match.");

        var user = await _uow.Users.GetByIdTrackedAsync(command.UserId, ct)
            ?? throw new NotFoundException("User", command.UserId);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        user.IsLocked = false;    // auto-unlock on admin reset
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }
}