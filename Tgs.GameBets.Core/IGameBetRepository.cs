using System.Text.Json.Nodes;

namespace Tgs.GameBets.Core;

public interface IGameBetRepository
{
	ValueTask AddAsync(Models.GameBetAddModel source, JsonNode additionalData, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<Models.GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<Models.GameBetPrimaryKeyModel?> GetPrimaryKeyAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(Models.GameBetUpdateModel source, JsonNode additionalData, CancellationToken cancellationToken = default);
}
