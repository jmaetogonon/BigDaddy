using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Features.Users.Commands.AssignTeams;

public class AssignTeamsCommandHandler : ICommandHandler<AssignTeamsCommand>
{
    private readonly IUnitOfWork _uow;

    public AssignTeamsCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(AssignTeamsCommand command, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdTrackedAsync(command.UserId, ct)
            ?? throw new NotFoundException("User", command.UserId);

        var existingTeamIds = user.UserTeams.Select(ut => ut.TeamId).ToHashSet();

        foreach (var teamId in command.TeamIds.Distinct().Where(t => !existingTeamIds.Contains(t)))
        {
            if (!await _uow.Users.TeamExistsAsync(teamId, ct))
                throw new NotFoundException("Team", teamId);

            _uow.Users.AddUserTeam(new UserTeam { UserId = command.UserId, TeamId = teamId });
        }

        await _uow.SaveAsync(ct);
    }
}