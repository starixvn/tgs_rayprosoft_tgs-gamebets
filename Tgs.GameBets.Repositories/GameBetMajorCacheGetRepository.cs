using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetMajors;

namespace Tgs.GameBets.Repositories;

internal class GameBetMajorCacheGetRepository(
	IGameBetMajorRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetMajorCacheGetRepository> logger) : IGameBetMajorRepository
{
	public ValueTask AddAsync(GameBetMajorAddModel source, CancellationToken cancellationToken = default)
		=> repository.AddAsync(source, cancellationToken);

	public ValueTask ClosedAsync(GameBetMajorClosedModel source, CancellationToken cancellationToken = default)
		=> repository.ClosedAsync(source, cancellationToken);

	public async ValueTask<GameBetMajorInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
	{
		try
		{
			var model = await GetFromCacheAsync(
				gameProviderId: gameProviderId,
				gameId: gameId,
				walletId: walletId,
				externalBetId: externalBetId)
				.ConfigureAwait(false);

			if (model is null)
			{
				model = await repository
					.GetAsync(
						externalCreatedOn: externalCreatedOn,
						gameProviderId: gameProviderId,
						gameId: gameId,
						walletId: walletId,
						externalBetId: externalBetId,
						cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				if (model is not null)
				{
					await SetToCacheAsync(
							gameProviderId: gameProviderId,
							gameId: gameId,
							walletId: walletId,
							externalBetId: externalBetId,
							model: model)
						.ConfigureAwait(false);
				}
			}

			return model;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					externalCreatedOn,
					gameProviderId,
					gameId,
					walletId,
					externalBetId,
					Message = "An exception has occurred when get bets to this game"
				}));

			throw;
		}
	}

	public async ValueTask<GameBetMajorInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default)
	{
		try
		{
			var model = await GetFromCacheAsync(
				id: id)
				.ConfigureAwait(false);

			if (model is null)
			{
				model = await repository
					.GetAsync(
						externalCreatedOn: externalCreatedOn,
						id: id,
						cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				if (model is not null)
				{
					await SetToCacheAsync(
							id: id,
							model: model)
						.ConfigureAwait(false);
				}
			}

			return model;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					externalCreatedOn,
					id,
					Message = "An exception has occurred when get bets to this game"
				}));

			throw;
		}
	}

	public async ValueTask ReopenAsync(GameBetMajorReopenModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			await DeleteCacheAsync(
					externalCreatedOn: source.ExternalCreatedOn,
					id: source.Id)
				.ConfigureAwait(false);

			await repository
				.ReopenAsync(
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
					source.ExternalCreatedOn,
					source.Id,
					Message = "An exception has occurred when reopen bets to this game"
				}));

			throw;
		}
	}

	public async ValueTask RunningAsync(GameBetMajorRunningModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			await DeleteCacheAsync(
					externalCreatedOn: source.ExternalCreatedOn,
					id: source.Id)
				.ConfigureAwait(false);

			await repository
				.RunningAsync(
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
					source.ExternalCreatedOn,
					source.Id,
					Message = "An exception has occurred when reopen bets to this game"
				}));

			throw;
		}
	}

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> string.Concat([
			typeof(GameBetMajorCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-major",
			":",
			"cache",
			":",
			"get",
			gameProviderId,
			":",
			gameId,
			":",
			walletId,
			":",
			externalBetId
		]);

	private static RedisKey GetKey(Guid id)
		=> string.Concat([
			typeof(GameBetMajorCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-major",
			":",
			"cache",
			":",
			"get",
			id
		]);

	private async ValueTask<GameBetMajorInfoModel?> GetFromCacheAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
	{
		var bytes = (byte[]?)await database
			.StringGetAsync(
				key: GetKey(gameProviderId, gameId, walletId, externalBetId),
				flags: CommandFlags.None)
			.ConfigureAwait(false);
		return bytes is null
			? null
			: JsonSerializer.Deserialize<GameBetMajorInfoModel>(bytes);
	}

	private async ValueTask<GameBetMajorInfoModel?> GetFromCacheAsync(Guid id)
	{
		var bytes = (byte[]?)await database
			.StringGetAsync(
				key: GetKey(id),
				flags: CommandFlags.None)
			.ConfigureAwait(false);
		return bytes is null
			? null
			: JsonSerializer.Deserialize<GameBetMajorInfoModel>(bytes);
	}

	private async ValueTask SetToCacheAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, GameBetMajorInfoModel model)
		=> await database
			.StringSetAsync(
				key: GetKey(gameProviderId, gameId, walletId, externalBetId),
				value: JsonSerializer.SerializeToUtf8Bytes(model),
				expiry: expiry,
				when: When.Always,
				flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);

	private async ValueTask SetToCacheAsync(Guid id, GameBetMajorInfoModel model)
		=> await database
			.StringSetAsync(
				key: GetKey(id),
				value: JsonSerializer.SerializeToUtf8Bytes(model),
				expiry: expiry,
				when: When.Always,
				flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);

	private async ValueTask DeleteCacheAsync(DateTimeOffset externalCreatedOn, Guid id)
		=> await database
			.KeyDeleteAsync(
				key: GetKey(id),
				flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
}
