namespace Tgs.GameBets.Client;

public interface IGameBetsClient
{
	ValueTask<Common.ViewModels.GameBetInfoViewModel> GameBetMajorCommandAsync(Common.ViewModels.GameBetMajorCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetMajorGetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetMajorGetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default);

	ValueTask<Common.Enums.GameBetCommandResult> GameBetClosesCommandAsync(Common.ViewModels.GameBetCloseCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetClosesGetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetClosesGetAsync(DateTimeOffset externalCreatedOn, Guid gameBetId, CancellationToken cancellationToken = default);

	ValueTask GameBetRunningCommandAsync(Common.ViewModels.GameBetRunningCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask GameBetRunningDeleteAsync(Guid gameBetId, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetRunningGetAsync(Guid gameBetId, CancellationToken cancellationToken = default);
}
