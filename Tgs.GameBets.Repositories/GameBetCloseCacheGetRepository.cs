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

	public async ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset closedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
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
					closedOn,
					gameProviderId,
					gameId,
					walletId,
					externalBetId,
					Message = "An exception has occurred when get bets to this game"
				}));
		}

		model = await repository
			.GetAsync(
				closedOn: closedOn,
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
						closedOn,
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

	public async ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset closedOn, Guid gameBetId, CancellationToken cancellationToken = default)
	{
		GameBetCloseInfoModel? model;
		bool cacheWorking = false;
		try
		{
			model = await GetFromCacheAsync(
					gameBetId: gameBetId)
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
					closedOn,
					gameBetId,
					Message = "An exception has occurred when get bets to this game"
				}));
		}

		model = await repository
			.GetAsync(
				closedOn: closedOn,
				gameBetId: gameBetId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (model is not null && cacheWorking)
		{
			try
			{
				await SetToCacheAsync(
						gameBetId: gameBetId,
						model: model)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "{logWarning}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						closedOn,
						gameBetId,
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
					gameBetId: source.GameBetId)
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
					source.ClosedOn,
					source.GameBetId,
					Message = "An exception has occurred when update bets to this game"
				}));

			throw;
		}
	}

	private static string GetKey(Guid gameBetId)
		=> string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			gameBetId
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

	private async ValueTask<GameBetCloseInfoModel?> GetFromCacheAsync(Guid gameBetId)
	{
		var json = (string?)await database
			.StringGetAsync(
				key: GetKey(gameBetId),
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

	private async ValueTask SetToCacheAsync(Guid gameBetId, GameBetCloseInfoModel model)
		=> await database
			.StringSetAsync(
				key: GetKey(gameBetId),
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

	private async ValueTask DeleteCacheAsync(Guid gameBetId)
		=> await database
			.KeyDeleteAsync(
				key: GetKey(gameBetId),
				flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
}
