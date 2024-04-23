using Microsoft.Extensions.Logging;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Core.ApplicationServices;

internal class GameBetFirstCommandService(
	ILogger<GameBetFirstCommandService> logger,
	IGameBetFirstRepository gameBetFirstRepository,
	ISerialNumberGenerator serialNumberGenerator) : ICommandService<GameBetFirstCommand>
{
	public async ValueTask ExecuteAsync(GameBetFirstCommand command, CancellationToken cancellationToken = default)
	{
		if (command.BetState is Common.Enums.BetState.Waiting or Common.Enums.BetState.Running)
		{
			logger.LogWarning("LogOn:{logOn} Message:The bet status of this game is not settled.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					command.GameProviderId,
					command.GameId,
					command.PlayerCode,
					command.WalletId,
					command.ExternalBetId,
					command.BetState
				});
			return;
		}

		if (await gameBetFirstRepository.ExistAsync(
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			return;
		}

		var id = await serialNumberGenerator.GenerateAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await gameBetFirstRepository.AddAsync(
			source: new Models.GameBetFirstAddModel(
				Id: id,
				GameBetId: command.GameBetId,
				GameProviderId: command.GameProviderId,
				GameProviderConfigurationId: command.GameProviderConfigurationId,
				GameId: command.GameId,
				ResellerId: command.ResellerId,
				LicenseeId: command.LicenseeId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				CurrencyCode: command.CurrencyCode,
				WalletId: command.WalletId,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: command.ExternalClosedOn,
				ExternalPayoffOn: command.ExternalPayoffOn,
				ExternalResettleOn: command.ExternalResettleOn,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				Jackpot: command.Jackpot,
				CreatedOn: command.CreatedOn,
				ClosedOn: command.ClosedOn,
				UpdatedOn: command.UpdatedOn,
				AdditionalData: command.AdditionalData),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
