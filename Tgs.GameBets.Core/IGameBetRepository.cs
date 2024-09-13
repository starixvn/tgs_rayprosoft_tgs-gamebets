using System.Text.Json.Nodes;
using Tgs.GameBets.Core.Models.GameBets;

namespace Tgs.GameBets.Core;

public interface IGameBetRepository
{
	ValueTask AddAsync(GameBetAddModel source, CancellationToken cancellationToken = default);

	ValueTask<GameBetInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<GameBetInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(GameBetUpdateModel source, CancellationToken cancellationToken = default);
}
