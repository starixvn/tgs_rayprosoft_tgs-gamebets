using Tgs.GameBets.Core.Models.GameBetMajors;
using Tgs.GameBets.Core.Models.GameBets;

namespace Tgs.GameBets.Core;
public interface IGameBetMajorRepository
{
	ValueTask AddAsync(GameBetMajorAddModel source, CancellationToken cancellationToken = default);

	ValueTask<GameBetMajorInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);

	ValueTask<GameBetMajorInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default);

	ValueTask ClosedAsync(GameBetMajorClosedModel source, CancellationToken cancellationToken = default);

	ValueTask ReopenAsync(GameBetMajorReopenModel source, CancellationToken cancellationToken = default);

	ValueTask RunningAsync(GameBetMajorRunningModel source, CancellationToken cancellationToken = default);
}