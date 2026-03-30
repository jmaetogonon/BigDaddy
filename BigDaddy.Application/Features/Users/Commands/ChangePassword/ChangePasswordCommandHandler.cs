using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;

namespace BigDaddy.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IUnitOfWork _uow;

    public ChangePasswordCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(ChangePasswordCommand command, CancellationToken ct = default)
    {
        if (command.NewPassword != command.ConfirmPassword)
            throw new ConflictException("New password and confirmation do not match.");

        var user = await _uow.Users.GetByIdTrackedAsync(command.UserId, ct)
            ?? throw new NotFoundException("User", command.UserId);

        if (!BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
            throw new ForbiddenException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }
}