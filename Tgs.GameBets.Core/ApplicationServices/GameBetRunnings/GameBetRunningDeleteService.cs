using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;

internal class GameBetRunningDeleteService(
	IGameBetRunningRepository repository) : IRequestHandler<GameBetRunningDeleteRequest>
{
	public async Task Handle(GameBetRunningDeleteRequest request, CancellationToken cancellationToken)
		=> await repository.DeleteAsync(
			gameBetId: request.GameBetId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
