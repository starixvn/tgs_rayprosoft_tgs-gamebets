using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetCloses;

namespace Tgs.GameBets.Repositories;

internal class GameBetCloseCacheGetRepository(
	IGameBetCloseRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetCloseCacheGetRepository> logger) : IGameBetCloseRepository
{
	public ValueTask AddAsync(GameBetCloseAddModel source, CancellationToken cancellationToken = default)
		=> repository.AddAsync(source, cancellationToken);

	public async ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
	{
		GameBetCloseInfoModel? model;
		bool cacheWorking = false;
		try
		{
			model = await GetFromCacheAsync(
					gameProviderId: gameProviderId,
					gameId: gameId,
					walletId: walletId,
					externalBetId: externalBetId)
				.ConfigureAwait(false);

			if (model is not null)
				return model;

			cacheWorking = true;
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "{logWarning}",
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
		}

		model = await repository
			.GetAsync(
				externalCreatedOn: externalCreatedOn,
				gameProviderId: gameProviderId,
				gameId: gameId,
				walletId: walletId,
				externalBetId: externalBetId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (model is not null && cacheWorking)
		{
			try
			{
				await SetToCacheAsync(
						gameProviderId: gameProviderId,
						gameId: gameId,
						walletId: walletId,
						externalBetId: externalBetId,
						model: model)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "{logWarning}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						externalCreatedOn,
						gameProviderId,
						gameId,
						walletId,
						externalBetId,
						Message = "An exception has occurred when cache bets to this game"
					}));
			}
		}

		return model;
	}

	public async ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameBetMajorId, CancellationToken cancellationToken = default)
	{
		GameBetCloseInfoModel? model;
		bool cacheWorking = false;
		try
		{
			model = await GetFromCacheAsync(
					gameBetMajorId: gameBetMajorId)
				.ConfigureAwait(false);

			if (model is not null)
				return model;

			cacheWorking = true;
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					externalCreatedOn,
					gameBetMajorId,
					Message = "An exception has occurred when get bets to this game"
				}));
		}

		model = await repository
			.GetAsync(
				externalCreatedOn: externalCreatedOn,
				gameBetMajorId: gameBetMajorId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (model is not null && cacheWorking)
		{
			try
			{
				await SetToCacheAsync(
						gameBetMajorId: gameBetMajorId,
						model: model)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "{logWarning}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						externalCreatedOn,
						gameBetMajorId,
						Message = "An exception has occurred when cache bets to this game"
					}));
			}
		}

		return model;
	}

	public async ValueTask UpdateAsync(GameBetCloseUpdateModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			await DeleteCacheAsync(
					gameBetMajorId: source.GameBetId)
				.ConfigureAwait(false);

			await repository
				.UpdateAsync(
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
					source.GameBetId,
					Message = "An exception has occurred when update bets to this game"
				}));

			throw;
		}
	}

	public ValueTask<Guid?> GetGameBetMajorIdAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
		=> repository.GetGameBetMajorIdAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalBetId, cancellationToken);

	private static string GetKey(Guid gameBetMajorId)
			=> string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			gameBetMajorId
		]);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
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

	private async ValueTask<GameBetCloseInfoModel?> GetFromCacheAsync(Guid gameBetMajorId)
	{
		var json = (string?)await database
			.StringGetAsync(
				key: GetKey(gameBetMajorId),
				flags: CommandFlags.None)
			.ConfigureAwait(false);
		return string.IsNullOrWhiteSpace(json)
			? null
			: JsonSerializer.Deserialize<GameBetCloseInfoModel>(json);
	}

	private async ValueTask<GameBetCloseInfoModel?> GetFromCacheAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
	{
		var json = (string?)await database
			.StringGetAsync(
				key: GetKey(gameProviderId, gameId, walletId, externalBetId),
				flags: CommandFlags.None)
			.ConfigureAwait(false);
		return string.IsNullOrWhiteSpace(json)
			? null
			: JsonSerializer.Deserialize<GameBetCloseInfoModel>(json);
	}

	private async ValueTask SetToCacheAsync(Guid gameBetMajorId, GameBetCloseInfoModel model)
		=> await database
			.StringSetAsync(
				key: GetKey(gameBetMajorId),
				value: JsonSerializer.Serialize(model),
				expiry: expiry,
				when: When.NotExists,
				flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);

	private async ValueTask SetToCacheAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, GameBetCloseInfoModel model)
		=> await database
			.StringSetAsync(
				key: GetKey(gameProviderId, gameId, walletId, externalBetId),
				value: JsonSerializer.Serialize(model),
				expiry: expiry,
				when: When.NotExists,
				flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);

	private async ValueTask DeleteCacheAsync(Guid gameBetMajorId)
		=> await database
			.KeyDeleteAsync(
				key: GetKey(gameBetMajorId),
				flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
}
