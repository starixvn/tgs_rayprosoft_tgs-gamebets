namespace Tgs.GameBets.Client;

public interface IGameBetsClient
{
	ValueTask<Common.Enums.GameBetCommandResult> GameBetCommandAsync(Common.ViewModels.GameBetCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetMajorInfoViewModel> GameBetMajorCommandAsync(Common.ViewModels.GameBetMajorCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetMajorInfoViewModel?> GameBetMajorGetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.GameBetMajorInfoViewModel?> GameBetMajorGetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default);
}
