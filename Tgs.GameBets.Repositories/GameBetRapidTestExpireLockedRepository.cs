using System.Text.Json.Nodes;
using StackExchange.Redis;
using Tgs.GameBets.Core;

namespace Tgs.GameBets.Repositories;

internal class GameBetRapidTestExpireLockedRepository(
	IDatabase database) : IGameBetRapidTestExpireLockedRepository
{
	public static string GetKey(string gameProviderCode, string application)
		=> string.Concat(typeof(GameBetRapidTestExpireLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-rapid-test", ":", "locked", ":", gameProviderCode, ":", application);

	public async ValueTask<bool> LockedAsync(string gameProviderCode, string application, CancellationToken cancellationToken = default)
		=> await database.StringSetAsync(
			key: GetKey(gameProviderCode, application),
			value: new JsonObject
			{
				["MachineName"] = Environment.MachineName,
				["GameProviderCode"] = gameProviderCode,
				["Application"] = application,
				["LabelTime"] = DateTimeOffset.Now
			}.ToJsonString(),
			expiry: TimeSpan.FromMinutes(10),
			when: When.NotExists)
		.ConfigureAwait(false);

	public async ValueTask UnlockAsync(string gameProviderCode, string application)
		=> _ = await database.KeyDeleteAsync(
			key: GetKey(gameProviderCode, application),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
