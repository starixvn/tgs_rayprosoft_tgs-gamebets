using Tgs.GameBets.Core.Models.GameBetCloses;

namespace Tgs.GameBets.Core;

public interface IGameBetCloseRepository
{
	ValueTask AddAsync(GameBetCloseAddModel source, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistsAsync(DateTimeOffset closedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset closedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset closedOn, Guid gameBetId, CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(GameBetCloseUpdateModel source, CancellationToken cancellationToken = default);
}
