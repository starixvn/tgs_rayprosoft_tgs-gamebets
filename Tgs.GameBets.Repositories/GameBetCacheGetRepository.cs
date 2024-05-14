using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models;

namespace Tgs.GameBets.Repositories;

internal class GameBetCacheGetRepository(
	IGameBetRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetCacheGetRepository> logger) : IGameBetRepository
{
	public ValueTask AddAsync(GameBetAddModel source, JsonNode additionalData, CancellationToken cancellationToken = default)
		=> repository.AddAsync(source, additionalData, cancellationToken);

	public ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.ExistAsync(gameProviderId, gameId, walletId, externalBetId, externalCreatedOn, cancellationToken);

	public async ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		var info = await StringGetAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId)
			.ConfigureAwait(false);

		if (info is null)
		{
			info = await repository.GetAsync(
				gameProviderId: gameProviderId,
				gameId: gameId,
				walletId: walletId,
				externalBetId: externalBetId,
				externalCreatedOn: externalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (info is not null)
			{
				await StringSetAsync(info).ConfigureAwait(false);
			}
		}

		return info;
	}

	public async ValueTask UpdateAsync(GameBetUpdateModel source, JsonNode additionalData, CancellationToken cancellationToken = default)
	{
		try
		{
			await repository.UpdateAsync(source, additionalData, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Message:An exception occurred when updating bet slips for this game.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					source.Id,
					source.GameProviderId,
					source.GameId,
					source.WalletId,
					source.ExternalBetId
				});
			throw;
		}
		finally
		{
			await KeyDeleteAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId)
				.ConfigureAwait(false);
		}
	}

	public ValueTask<GameBetPrimaryKeyModel?> GetPrimaryKeyAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetPrimaryKeyAsync(id, externalCreatedOn, cancellationToken);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
			=> string.Concat(typeof(GameBetCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "Cache", ":", "get", gameProviderId, ":", gameId, ":", walletId, ":", externalBetId);

	private async ValueTask<GameBetInfoModel?> StringGetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
	{
		if (await database.KeyExistsAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId))
			.ConfigureAwait(false))
		{
			var value = await database.StringGetAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId))
			.ConfigureAwait(false);

			return JsonSerializer.Deserialize<GameBetInfoModel>(value.ToString());
		}

		return null;
	}

	private async ValueTask<bool> StringSetAsync(GameBetInfoModel source)
		=> await database.StringSetAsync(
			key: GetKey(source.GameProviderId, source.GameId, source.WalletId, source.ExternalBetId),
			value: JsonSerializer.Serialize(source),
			when: When.Always,
			expiry: expiry)
		.ConfigureAwait(false);

	private async ValueTask KeyDeleteAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
