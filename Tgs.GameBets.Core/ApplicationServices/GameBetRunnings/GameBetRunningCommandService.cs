using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Core.Models.GameBetRunnings;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;

internal class GameBetRunningCommandService(
	ILogger<GameBetRunningCommandService> logger,
	IGameBetRunningRepository repository) : IRequestHandler<GameBetRunningCommandRequest>
{
	public async Task Handle(GameBetRunningCommandRequest request, CancellationToken cancellationToken)
	{
		var info = await repository.GetAsync(
			gameBetId: request.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (info is null)
		{
			await repository.AddAsync(
				source: new GameBetRunningAddModel(
					GameBetId: request.GameBetId,
					GameProviderId: request.GameProviderId,
					GameProviderConfigurationId: request.GameProviderConfigurationId,
					GameId: request.GameId,
					ResellerId: request.ResellerId,
					LicenseeId: request.LicenseeId,
					BrandId: request.BrandId,
					PlayerCode: request.PlayerCode,
					CurrencyCode: request.CurrencyCode,
					WalletId: request.WalletId,
					ExternalRoundId: request.ExternalRoundId,
					ExternalBetId: request.ExternalBetId,
					ExternalCreatedOn: request.ExternalCreatedOn,
					BetType: request.BetType,
					CurrencyRatio: request.CurrencyRatio,
					Bet: request.Bet,
					ValidBet: request.ValidBet,
					CreatedOn: request.CreatedOn,
					AdditionalData: request.AdditionalData,
					ExtendedData: request.ExtendedData),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			if (
				info.CurrencyRatio != request.CurrencyRatio ||
				info.Bet != request.Bet ||
				info.ValidBet != request.ValidBet)
			{
				logger.LogWarning("{logWarning}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						info.GameBetId,
						info.GameProviderId,
						info.GameId,
						info.PlayerCode,
						info.WalletId,
						info.ExternalBetId,
						ExternalCreatedOn = info.ExternalCreatedOn.ToString("u"),
						CurrentCurrencyRatio = info.CurrencyRatio,
						CurrentBet = info.Bet,
						CurrentValidBet = info.ValidBet,
						RequestCurrencyRatio = request.CurrencyRatio,
						RequestBet = request.Bet,
						RequestValidBet = request.ValidBet,
						Message = "This game bet has been changed and the settlement statement has been changed."
					}));

				await repository.UpdateAsync(
					source: new GameBetRunningUpdateModel(
						GameBetId: info.GameBetId,
						BetType: request.BetType,
						CurrencyRatio: request.CurrencyRatio,
						Bet: request.Bet,
						ValidBet: request.ValidBet,
						AdditionalData: request.AdditionalData,
						ExtendedData: request.ExtendedData),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
		}
	}
}
