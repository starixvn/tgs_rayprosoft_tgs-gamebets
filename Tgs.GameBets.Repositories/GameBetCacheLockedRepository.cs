using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models;

namespace Tgs.GameBets.Repositories;

internal class GameBetCacheLockedRepository(
	IGameBetRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetCacheLockedRepository> logger) : IGameBetRepository
{
	public async ValueTask AddAsync(GameBetAddModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await SetKeyAsync(
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
			await DeleteKeyAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId)
				.ConfigureAwait(false);

			throw;
		}
	}

	public ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	=> repository.ExistAsync(gameProviderId, gameId, walletId, externalBetId, externalCreatedOn, cancellationToken);

	public ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameProviderId, gameId, walletId, externalBetId, externalCreatedOn, cancellationToken);

	public ValueTask UpdateAsync(GameBetUpdateModel source, CancellationToken cancellationToken = default)
		=> repository.UpdateAsync(source, cancellationToken);

	public ValueTask<GameBetPrimaryKeyModel?> GetPrimaryKeyAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetPrimaryKeyAsync(id, externalCreatedOn, cancellationToken);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
			=> string.Concat(typeof(GameBetCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "Cache", ":", "locked", gameProviderId, ":", gameId, ":", walletId, ":", externalBetId);

	private async ValueTask<bool> SetKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn)
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

	private async ValueTask DeleteKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
