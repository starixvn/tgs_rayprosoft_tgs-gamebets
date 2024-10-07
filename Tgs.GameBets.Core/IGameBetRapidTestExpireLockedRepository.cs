namespace Tgs.GameBets.Core;

public interface IGameBetRapidTestExpireLockedRepository
{
	public ValueTask<bool> LockedAsync(string gameProviderCode, string application, CancellationToken cancellationToken = default);

	public ValueTask UnlockAsync(string gameProviderCode, string application);
}
