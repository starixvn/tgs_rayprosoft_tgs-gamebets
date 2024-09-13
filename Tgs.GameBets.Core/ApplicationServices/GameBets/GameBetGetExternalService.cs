using System.Text.Json.Nodes;
using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBets;

internal class GameBetGetExternalService(
	IGameBetRepository repository) : IRequestHandler<GameBetGetExternalRequest, GameBetInfoResponse?>
{
	public async Task<GameBetInfoResponse?> Handle(GameBetGetExternalRequest request, CancellationToken cancellationToken)
	{
		var gameBetInfo = await repository.GetAsync(
			externalCreatedOn: request.ExternalCreatedOn,
			gameProviderId: request.GameProviderId,
			gameId: request.GameId,
			walletId: request.WalletId,
			externalBetId: request.ExternalBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return gameBetInfo is null
			? null
			: new GameBetInfoResponse(
				Id: gameBetInfo.Id,
				GameProviderId: gameBetInfo.GameProviderId,
				GameProviderConfigurationId: gameBetInfo.GameProviderConfigurationId,
				GameId: gameBetInfo.GameId,
				ResellerId: gameBetInfo.ResellerId,
				LicenseeId: gameBetInfo.LicenseeId,
				BrandId: gameBetInfo.BrandId,
				PlayerCode: gameBetInfo.PlayerCode,
				CurrencyCode: gameBetInfo.CurrencyCode,
				WalletId: gameBetInfo.WalletId,
				ExternalRoundId: gameBetInfo.ExternalRoundId,
				ExternalBetId: gameBetInfo.ExternalBetId,
				ExternalCreatedOn: gameBetInfo.ExternalCreatedOn,
				ExternalClosedOn: gameBetInfo.ExternalClosedOn,
				ExternalPayoffOn: gameBetInfo.ExternalPayoffOn,
				ExternalResettleOn: gameBetInfo.ExternalResettleOn,
				BetState: gameBetInfo.BetState,
				BetType: gameBetInfo.BetType,
				FreeGameState: gameBetInfo.FreeGameState,
				GambleState: gameBetInfo.GambleState,
				CashoutState: gameBetInfo.CashoutState,
				ResettleState: gameBetInfo.ResettleState,
				JackpotState: gameBetInfo.JackpotState,
				CurrencyRatio: gameBetInfo.CurrencyRatio,
				Bet: gameBetInfo.Bet,
				ValidBet: gameBetInfo.ValidBet,
				Settle: gameBetInfo.Settle,
				Commission: gameBetInfo.Commission,
				Turnover: gameBetInfo.Turnover,
				WinLoss: gameBetInfo.WinLoss,
				Jackpot: gameBetInfo.Jackpot,
				CreatedOn: gameBetInfo.CreatedOn,
				ClosedOn: gameBetInfo.ClosedOn,
				UpdatedOn: gameBetInfo.UpdatedOn,
				ResetState: gameBetInfo.ResetState,
				ResetOn: gameBetInfo.ResetOn,
				AdditionalData: JsonNode.Parse(gameBetInfo.AdditionalData) ?? new JsonObject { });
	}
}
