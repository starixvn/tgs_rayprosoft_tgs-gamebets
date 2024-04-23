namespace Tgs.GameBets.Core;

public interface IGameBetFirstRepository
{
	ValueTask AddAsync(Models.GameBetFirstAddModel source, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);
}
