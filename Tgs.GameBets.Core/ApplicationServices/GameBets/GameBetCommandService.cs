using System.Text.Json;
using System.Text.Json.Nodes;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Core.Models.GameBets;

namespace Tgs.GameBets.Core.ApplicationServices.GameBets;

internal class GameBetCommandService(
	ILogger<GameBetCommandService> logger,
	IGameBetRepository repository) : IRequestHandler<GameBetCommandRequest, Common.Enums.GameBetCommandResult>
{
	public async Task<Common.Enums.GameBetCommandResult> Handle(GameBetCommandRequest request, CancellationToken cancellationToken)
	{
		if (request.BetState is Common.Enums.BetState.Waiting or Common.Enums.BetState.Running)
		{
			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
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

		var gameBetInfo = await repository.GetAsync(
			gameProviderId: request.GameProviderId,
			gameId: request.GameId,
			walletId: request.WalletId,
			externalBetId: request.ExternalBetId,
			externalCreatedOn: request.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (gameBetInfo is null)
		{
			await repository.AddAsync(
				source: new GameBetAddModel(
					Id: request.GameBetId,
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
					Commission: request.Commission,
					Turnover: request.Turnover,
					WinLoss: request.WinLoss,
					Jackpot: request.Jackpot,
					AdditionalData: JsonNode.Parse(request.AdditionalData)!),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return Common.Enums.GameBetCommandResult.Add;
		}
		else
		{
			if (
				gameBetInfo.BetState == request.BetState &&
				gameBetInfo.BetType == request.BetType &&
				gameBetInfo.FreeGameState == request.FreeGameState &&
				gameBetInfo.GambleState == request.GambleState &&
				gameBetInfo.CashoutState == request.CashoutState &&
				gameBetInfo.ResettleState == request.ResettleState &&
				gameBetInfo.JackpotState == request.JackpotState &&
				gameBetInfo.Bet == request.Bet &&
				gameBetInfo.ValidBet == request.ValidBet &&
				gameBetInfo.Settle == request.Settle &&
				gameBetInfo.Commission == request.Commission &&
				gameBetInfo.Turnover == request.Turnover &&
				gameBetInfo.WinLoss == request.WinLoss &&
				gameBetInfo.Jackpot == request.Jackpot)
			{
				return Common.Enums.GameBetCommandResult.None;
			}

			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					gameBetInfo.Id,
					gameBetInfo.GameProviderId,
					gameBetInfo.GameId,
					gameBetInfo.PlayerCode,
					gameBetInfo.WalletId,
					gameBetInfo.ExternalBetId,
					ExternalCreatedOn = gameBetInfo.ExternalCreatedOn.ToString("u"),
					CurrentBetState = gameBetInfo.BetState.ToString(),
					RequestBetState = request.BetState.ToString(),
					Message = "This game's betting data has changed."
				}));

			await repository.UpdateAsync(
				source: new GameBetUpdateModel(
					Id: gameBetInfo.Id,
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
					Commission: request.Commission,
					Turnover: request.Turnover,
					WinLoss: request.WinLoss,
					Jackpot: request.Jackpot,
					AdditionalData: JsonNode.Parse(request.AdditionalData)!),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return Common.Enums.GameBetCommandResult.Update;
		}
	}
}
