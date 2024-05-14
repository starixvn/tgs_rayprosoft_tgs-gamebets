using System.Text.Json.Nodes;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models;

namespace Tgs.GameBets.Repositories;

internal class GameBetCacheExistRepository(
	IGameBetRepository repository,
	IDatabase database,
	TimeSpan expiry) : IGameBetRepository
{
	public async ValueTask AddAsync(GameBetAddModel source, JsonNode additionalData, CancellationToken cancellationToken = default)
	{
		await repository.AddAsync(source, additionalData, cancellationToken).ConfigureAwait(false);

		await StringSetAsync(
			gameProviderId: source.GameProviderId,
			gameId: source.GameId,
			walletId: source.WalletId,
			externalBetId: source.ExternalBetId,
			externalCreatedOn: source.ExternalCreatedOn)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		if (!await KeyExistsAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId)
			.ConfigureAwait(false))
		{
			var exist = await repository.ExistAsync(
				gameProviderId: gameProviderId,
				gameId: gameId,
				walletId: walletId,
				externalBetId: externalBetId,
				externalCreatedOn: externalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (exist)
			{
				await StringSetAsync(
					gameProviderId: gameProviderId,
					gameId: gameId,
					walletId: walletId,
					externalBetId: externalBetId,
					externalCreatedOn: externalCreatedOn)
					.ConfigureAwait(false);
			}

			return exist;
		}

		return true;
	}

	public ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameProviderId, gameId, walletId, externalBetId, externalCreatedOn, cancellationToken);

	public ValueTask UpdateAsync(GameBetUpdateModel source, JsonNode additionalData, CancellationToken cancellationToken = default)
		=> repository.UpdateAsync(source, additionalData, cancellationToken);

	public ValueTask<GameBetPrimaryKeyModel?> GetPrimaryKeyAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetPrimaryKeyAsync(id, externalCreatedOn, cancellationToken);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> string.Concat(typeof(GameBetCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "Cache", ":", "exist", gameProviderId, ":", gameId, ":", walletId, ":", externalBetId);

	private async ValueTask<bool> KeyExistsAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> await database.KeyExistsAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId))
		.ConfigureAwait(false);

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
			when: When.Always,
			expiry: expiry)
		.ConfigureAwait(false);
}
