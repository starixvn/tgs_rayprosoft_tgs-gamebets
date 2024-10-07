using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetRunnings;

namespace Tgs.GameBets.Repositories;

internal class GameBetRunningCacheLockedRepository(
	IGameBetRunningRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetRunningCacheLockedRepository> logger) : IGameBetRunningRepository
{
	public async ValueTask AddAsync(GameBetRunningAddModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await SetKeyAsync(
				externalCreatedOn: source.ExternalCreatedOn,
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId)
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
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					source.GameBetId,
					source.GameProviderId,
					source.GameId,
					source.PlayerCode,
					source.CurrencyCode,
					source.WalletId,
					source.ExternalBetId,
					ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
					Message = "An exception has occurred when adding bets to this game"
				}));

			await DeleteKeyAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId)
				.ConfigureAwait(false);

			throw;
		}
	}

	public ValueTask DeleteAsync(Guid gameBetId, CancellationToken cancellationToken = default)
		=> repository.DeleteAsync(gameBetId, cancellationToken);

	public ValueTask<GameBetRunningInfoModel?> GetAsync(Guid gameBetId, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameBetId, cancellationToken);

	public ValueTask UpdateAsync(GameBetRunningUpdateModel source, CancellationToken cancellationToken = default)
		=> repository.UpdateAsync(source, cancellationToken);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> string.Concat(typeof(GameBetRunningCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-running", ":", "cache", ":", "locked", gameProviderId, ":", gameId, ":", walletId, ":", externalBetId);

	private async ValueTask<bool> SetKeyAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
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
			expiry: expiry,
			when: When.NotExists)
		.ConfigureAwait(false);

	private async ValueTask DeleteKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
