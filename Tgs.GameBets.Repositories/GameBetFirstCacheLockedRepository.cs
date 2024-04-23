using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Core;

namespace Tgs.GameBets.Repositories;

internal class GameBetFirstCacheLockedRepository(
	IGameBetFirstRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetFirstCacheLockedRepository> logger) : IGameBetFirstRepository
{
	public async ValueTask AddAsync(Core.Models.GameBetFirstAddModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await StringSetAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId,
				externalCreatedOn: source.ExternalCreatedOn)
				.ConfigureAwait(false))
			{
				return;
			}

			await repository.AddAsync(
				source: source,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Message:An exception has occurred when adding bets to this game..{info}",
				DateTime.Now.ToString("o"),
				new
				{
					source.Id,
					source.GameProviderId,
					source.GameId,
					source.WalletId,
					source.PlayerCode,
					source.ExternalBetId
				});
			await KeyDeleteAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId)
				.ConfigureAwait(false);

			throw;
		}
	}

	public ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.ExistAsync(gameProviderId: gameProviderId, gameId: gameId, walletId: walletId, externalBetId: externalBetId, externalCreatedOn: externalCreatedOn, cancellationToken: cancellationToken);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> string.Concat(typeof(GameBetFirstCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-first", ":", "cache", ":", "locked", gameProviderId, ":", gameId, ":", walletId, ":", externalBetId);

	private async ValueTask<bool> StringSetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn)
		=> await database.StringSetAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId),
			value: new JsonObject
			{
				["GameProviderId"] = gameProviderId,
				["GameId"] = gameId,
				["WalletId"] = walletId,
				["ExternalBetId"] = externalBetId,
				["ExternalCreatedOn"] = externalCreatedOn,
				["LockedOn"] = DateTimeOffset.UtcNow
			}.ToJsonString(),
			when: When.NotExists,
			expiry: expiry)
		.ConfigureAwait(false);

	private async ValueTask KeyDeleteAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
