using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;

internal class GameBetRapidTestInitializationCommandService(
	IGameBetRapidTestRepository repository) : IRequestHandler<GameBetRapidTestInitializationCommandRequest>
{
	public Task Handle(GameBetRapidTestInitializationCommandRequest request, CancellationToken cancellationToken)
		=> repository.InitializationAsync(request.GameProviderCode, cancellationToken).AsTask();
}
