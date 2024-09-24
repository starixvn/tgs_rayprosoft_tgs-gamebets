using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Core.Models.GameBetCloses;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetCloses;

internal class GameBetCloseCommandService(
	ILogger<GameBetCloseCommandService> logger,
	ISerialNumberGenerator serialNumberGenerator,
	IGameBetCloseRepository repository) : IRequestHandler<GameBetCloseCommandRequest, Common.Enums.GameBetCommandResult>
{
	public async Task<Common.Enums.GameBetCommandResult> Handle(GameBetCloseCommandRequest request, CancellationToken cancellationToken)
	{
		if (request.BetState is Common.Enums.BetState.Waiting or Common.Enums.BetState.Running)
		{
			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					request.GameBetId,
					request.GameProviderId,
					request.GameId,
					request.PlayerCode,
					request.WalletId,
					request.ExternalBetId,
					ExternalCreatedOn = request.ExternalCreatedOn.ToString("u"),
					RequestBetState = request.BetState.ToString(),
					Message = "The bet status of this game is not settled."
				}));

			return Common.Enums.GameBetCommandResult.Running;
		}

		var info = await repository.GetAsync(
			closedOn: request.ExternalCreatedOn,
			gameProviderId: request.GameProviderId,
			gameId: request.GameId,
			walletId: request.WalletId,
			externalBetId: request.ExternalBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (info is null)
		{
			var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);

			await repository.AddAsync(
				source: new GameBetCloseAddModel(
					Id: id,
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
					ExternalClosedOn: request.ExternalClosedOn,
					ExternalPayoffOn: request.ExternalPayoffOn,
					ExternalResettleOn: request.ExternalResettleOn,
					BetState: request.BetState,
					BetType: request.BetType,
					FreeGameState: request.FreeGameState,
					GambleState: request.GambleState,
					CashoutState: request.CashoutState,
					ResettleState: request.ResettleState,
					JackpotState: request.JackpotState,
					CurrencyRatio: request.CurrencyRatio,
					Bet: request.Bet,
					ValidBet: request.ValidBet,
					Settle: request.Settle,
					WinLoss: request.WinLoss,
					Turnover: request.Turnover,
					Commission: request.Commission,
					Contribute: request.Contribute,
					Jackpot: request.Jackpot,
					CreatedOn: request.CreatedOn,
					ClosedOn: request.ClosedOn,
					AdditionalData: request.AdditionalData,
					ExtendedData: request.ExtendedData),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return Common.Enums.GameBetCommandResult.Add;
		}
		else
		{
			if (
				info.BetState == request.BetState &&
				info.BetType == request.BetType &&
				info.FreeGameState == request.FreeGameState &&
				info.GambleState == request.GambleState &&
				info.CashoutState == request.CashoutState &&
				info.ResettleState == request.ResettleState &&
				info.JackpotState == request.JackpotState &&
				info.CurrencyRatio == request.CurrencyRatio &&
				info.Bet == request.Bet &&
				info.ValidBet == request.ValidBet &&
				info.Settle == request.Settle &&
				info.WinLoss == request.WinLoss &&
				info.Turnover == request.Turnover &&
				info.Commission == request.Commission &&
				info.Contribute == request.Contribute &&
				info.Jackpot == request.Jackpot)
			{
				return Common.Enums.GameBetCommandResult.None;
			}

			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					info.Id,
					info.GameBetId,
					info.GameProviderId,
					info.GameId,
					info.PlayerCode,
					info.WalletId,
					info.ExternalBetId,
					ExternalCreatedOn = info.ExternalCreatedOn.ToString("u"),
					IsBetState = info.BetState == request.BetState,
					IsBetType = info.BetType == request.BetType,
					IsFreeGameState = info.FreeGameState == request.FreeGameState,
					IsGambleState = info.GambleState == request.GambleState,
					IsCashoutState = info.CashoutState == request.CashoutState,
					IsResettleState = info.ResettleState == request.ResettleState,
					IsJackpotState = info.JackpotState == request.JackpotState,
					IsCurrencyRatio = info.CurrencyRatio == request.CurrencyRatio,
					IsBet = info.Bet == request.Bet,
					IsValidBet = info.ValidBet == request.ValidBet,
					IsSettle = info.Settle == request.Settle,
					IsWinLoss = info.WinLoss == request.WinLoss,
					IsTurnover = info.Turnover == request.Turnover,
					IsCommission = info.Commission == request.Commission,
					IsContribute = info.Contribute == request.Contribute,
					IsJackpot = info.Jackpot == request.Jackpot,
					Message = "This game's betting data has changed."
				}));

			await repository.UpdateAsync(
				source: new GameBetCloseUpdateModel(
					GameBetId: info.GameBetId,
					ClosedOn: info.ExternalCreatedOn,
					ExternalClosedOn: request.ExternalClosedOn,
					ExternalPayoffOn: request.ExternalPayoffOn,
					ExternalResettleOn: request.ExternalResettleOn,
					BetState: request.BetState,
					BetType: request.BetType,
					FreeGameState: request.FreeGameState,
					GambleState: request.GambleState,
					CashoutState: request.CashoutState,
					ResettleState: request.ResettleState,
					JackpotState: request.JackpotState,
					CurrencyRatio: request.CurrencyRatio,
					Bet: request.Bet,
					ValidBet: request.ValidBet,
					Settle: request.Settle,
					WinLoss: request.WinLoss,
					Turnover: request.Turnover,
					Commission: request.Commission,
					Contribute: request.Contribute,
					Jackpot: request.Jackpot,
					AdditionalData: request.AdditionalData,
					ExtendedData: request.ExtendedData),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return Common.Enums.GameBetCommandResult.Update;
		}
	}
}
