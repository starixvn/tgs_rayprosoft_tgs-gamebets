using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Core.ApplicationServices;

internal class GameBetCommandService(
	ILogger<GameBetCommandService> logger,
	IGameBetRepository repository,
	ICommandService<GameBetAddCommand> addService,
	ICommandService<GameBetUpdateCommand> updateService) : IGetService<GameBetCommand, Common.Enums.GameBetCommandResult>
{

	public async ValueTask<Common.Enums.GameBetCommandResult> GetAsync(GameBetCommand query, CancellationToken cancellationToken = default)
	{
		if (query.BetState is Common.Enums.BetState.Waiting or Common.Enums.BetState.Running)
		{
			logger.LogWarning("LogOn:{logOn} Message:The bet status of this game is not settled.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					query.GameProviderId,
					query.GameId,
					query.PlayerCode,
					query.WalletId,
					query.ExternalBetId,
					query.BetState
				});
			return Common.Enums.GameBetCommandResult.Running;
		}

		var model = await repository.GetAsync(
			gameProviderId: query.GameProviderId,
			gameId: query.GameId,
			walletId: query.WalletId,
			externalBetId: query.ExternalBetId,
			externalCreatedOn: query.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (model is null)
		{
			await addService.ExecuteAsync(
				command: new GameBetAddCommand(
					GameBetId: query.GameBetId,
					GameProviderId: query.GameProviderId,
					GameProviderConfigurationId: query.GameProviderConfigurationId,
					GameId: query.GameId,
					ResellerId: query.ResellerId,
					LicenseeId: query.LicenseeId,
					BrandId: query.BrandId,
					PlayerCode: query.PlayerCode,
					CurrencyCode: query.CurrencyCode,
					WalletId: query.WalletId,
					ExternalRoundId: query.ExternalRoundId,
					ExternalBetId: query.ExternalBetId,
					ExternalCreatedOn: query.ExternalCreatedOn,
					ExternalClosedOn: query.ExternalClosedOn,
					ExternalPayoffOn: query.ExternalPayoffOn,
					ExternalResettleOn: query.ExternalResettleOn,
					BetState: query.BetState,
					BetType: query.BetType,
					FreeGameState: query.FreeGameState,
					GambleState: query.GambleState,
					CashoutState: query.CashoutState,
					ResettleState: query.ResettleState,
					JackpotState: query.JackpotState,
					CurrencyRatio: query.CurrencyRatio,
					Bet: query.Bet,
					ValidBet: query.ValidBet,
					Settle: query.Settle,
					Commission: query.Commission,
					Turnover: query.Turnover,
					WinLoss: query.WinLoss,
					Jackpot: query.Jackpot,
					AdditionalData: query.AdditionalData),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return Common.Enums.GameBetCommandResult.Add;
		}
		else
		{
			if (
				model.BetState == query.BetState &&
				model.BetType == query.BetType &&
				model.FreeGameState == query.FreeGameState &&
				model.GambleState == query.GambleState &&
				model.CashoutState == query.CashoutState &&
				model.ResettleState == query.ResettleState &&
				model.JackpotState == query.JackpotState &&
				model.Bet == query.Bet &&
				model.ValidBet == query.ValidBet &&
				model.Settle == query.Settle &&
				model.Commission == query.Commission &&
				model.Turnover == query.Turnover &&
				model.WinLoss == query.WinLoss &&
				model.Jackpot == query.Jackpot)
			{
				return Common.Enums.GameBetCommandResult.None;
			}

			logger.LogWarning("LogOn:{logOn} Message:The content of this Game Wager order is different, please change the Game Wager.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					model.Id,
					query.GameProviderId,
					query.GameId,
					query.PlayerCode,
					query.WalletId,
					query.ExternalBetId,
				});

			await updateService.ExecuteAsync(
				command: new GameBetUpdateCommand(
					GameBetId: model.Id,
					GameProviderId: query.GameProviderId,
					GameId: query.GameId,
					ResellerId: query.ResellerId,
					LicenseeId: query.LicenseeId,
					BrandId: query.BrandId,
					PlayerCode: query.PlayerCode,
					CurrencyCode: query.CurrencyCode,
					WalletId: query.WalletId,
					ExternalRoundId: query.ExternalRoundId,
					ExternalBetId: query.ExternalBetId,
					ExternalCreatedOn: query.ExternalCreatedOn,
					ExternalResettleOn: query.ExternalResettleOn,
					BetState: query.BetState,
					BetType: query.BetType,
					FreeGameState: query.FreeGameState,
					GambleState: query.GambleState,
					CashoutState: query.CashoutState,
					ResettleState: query.ResettleState,
					JackpotState: query.JackpotState,
					CurrencyRatio: query.CurrencyRatio,
					Bet: query.Bet,
					ValidBet: query.ValidBet,
					Settle: query.Settle,
					Commission: query.Commission,
					Turnover: query.Turnover,
					WinLoss: query.WinLoss,
					Jackpot: query.Jackpot,
					AdditionalData: query.AdditionalData),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return Common.Enums.GameBetCommandResult.Update;
		}
	}
}
