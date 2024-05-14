using System.Text.Json;
using System.Text.Json.Nodes;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models;

namespace Tgs.GameBets.Repositories;

internal class GameBetCachePrimaryKeyRepository(
	IGameBetRepository repository,
	IDatabase database,
	TimeSpan expiry) : IGameBetRepository
{
	public ValueTask AddAsync(GameBetAddModel source, JsonNode additionalData, CancellationToken cancellationToken = default)
	=> repository.AddAsync(source, additionalData, cancellationToken);

	public ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.ExistAsync(gameProviderId, gameId, walletId, externalBetId, externalCreatedOn, cancellationToken);

	public ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameProviderId, gameId, walletId, externalBetId, externalCreatedOn, cancellationToken);

	public ValueTask UpdateAsync(GameBetUpdateModel source, JsonNode additionalData, CancellationToken cancellationToken = default)
		=> repository.UpdateAsync(source, additionalData, cancellationToken);

	public async ValueTask<GameBetPrimaryKeyModel?> GetPrimaryKeyAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		var primaryKey = await StringGetAsync(id: id)
			.ConfigureAwait(false);

		if (primaryKey is null)
		{
			primaryKey = await repository.GetPrimaryKeyAsync(
				id: id,
				externalCreatedOn: externalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (primaryKey is not null)
			{
				await StringSetAsync(primaryKey).ConfigureAwait(false);
			}
		}

		return primaryKey;
	}

	private static string GetKey(Guid id)
		=> string.Concat(typeof(GameBetCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "Cache", ":", "pk", id);

	private async ValueTask<GameBetPrimaryKeyModel?> StringGetAsync(Guid id)
	{
		if (await database.KeyExistsAsync(
			key: GetKey(id))
			.ConfigureAwait(false))
		{
			var value = await database.StringGetAsync(
				key: GetKey(id))
				.ConfigureAwait(false);

			return JsonSerializer.Deserialize<GameBetPrimaryKeyModel>(value.ToString());
		}

		return null;
	}

	private async ValueTask<bool> StringSetAsync(GameBetPrimaryKeyModel source)
		=> await database.StringSetAsync(
			key: GetKey(source.Id),
			value: JsonSerializer.Serialize(source),
			when: When.Always,
			expiry: expiry)
		.ConfigureAwait(false);
}
