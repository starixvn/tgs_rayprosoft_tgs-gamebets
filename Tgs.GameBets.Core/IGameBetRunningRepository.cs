using Tgs.GameBets.Core.Models.GameBetRunnings;

namespace Tgs.GameBets.Core;

public interface IGameBetRunningRepository
{
	ValueTask AddAsync(GameBetRunningAddModel source, CancellationToken cancellationToken = default);

	ValueTask DeleteAsync(Guid gameBetId, CancellationToken cancellationToken = default);

	ValueTask<GameBetRunningInfoModel?> GetAsync(Guid gameBetId, CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(GameBetRunningUpdateModel source, CancellationToken cancellationToken = default);
}
