using Tgs.GameBets.Core.Models.GameBetRapidTests;

namespace Tgs.GameBets.Core;

public interface IGameBetRapidTestRepository
{
	ValueTask<bool> MergeAsync(GameBetRapidTestMerge source, CancellationToken cancellationToken = default);

	ValueTask<long> SetExpireOnAsync(string gameProviderCode, CancellationToken cancellationToken = default);

	ValueTask InitializationAsync(string gameProviderCode, CancellationToken cancellationToken = default);
}
