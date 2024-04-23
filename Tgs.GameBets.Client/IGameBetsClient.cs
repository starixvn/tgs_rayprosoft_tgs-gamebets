namespace Tgs.GameBets.Client;

public interface IGameBetsClient
{
	ValueTask GameBetCommandAsync(Common.ViewModels.GameBetCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetByExternalBetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask GameBetFirstCommandAsync(Common.ViewModels.GameBetFirstCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask<bool> GameBetFirstExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);
}
