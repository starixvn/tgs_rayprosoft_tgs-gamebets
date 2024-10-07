using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;

internal class GameBetRapidTestExpireCommandService(
	ILogger<GameBetRapidTestExpireCommandService> logger,
	IGameBetRapidTestExpireLockedRepository gameWagersLock,
	IGameBetRapidTestRepository repository) : IRequestHandler<GameBetRapidTestExpireCommandRequest>
{
	public async Task Handle(GameBetRapidTestExpireCommandRequest request, CancellationToken cancellationToken)
	{
		if (await gameWagersLock.LockedAsync(
			gameProviderCode: request.GameProviderCode,
			application: typeof(GameBetRapidTestExpireCommandService).Name,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			try
			{
				var momodifiedCount = await repository.SetExpireOnAsync(
					gameProviderCode: request.GameProviderCode,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				if (momodifiedCount > 0)
				{
					logger.LogWarning("{logWarning}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							request.GameProviderCode,
							Count = momodifiedCount,
							Message = "遊戲供應商設定到期時間筆數"
						}));
				}
				else
				{
					logger.LogInformation("{logInformation}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							request.GameProviderCode,
							Count = momodifiedCount,
							Message = "遊戲供應商設定到期時間筆數"
						}));
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{logError}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						request.GameProviderCode,
						ex.Message
					}));
				throw;
			}
			finally
			{
				await gameWagersLock.UnlockAsync(
					gameProviderCode: request.GameProviderCode,
					application: typeof(GameBetRapidTestExpireCommandService).Name)
					.ConfigureAwait(false);
			}
		}
	}
}
