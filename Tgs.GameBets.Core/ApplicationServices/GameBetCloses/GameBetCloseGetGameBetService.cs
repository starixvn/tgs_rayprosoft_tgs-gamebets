using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetCloses;

internal class GameBetCloseGetGameBetService(
	IGameBetCloseRepository repository) : IRequestHandler<GameBetCloseGetGameBetRequest, GameBetCloseInfoResponse?>
{
	public async Task<GameBetCloseInfoResponse?> Handle(GameBetCloseGetGameBetRequest request, CancellationToken cancellationToken)
	{
		var info = await repository.GetAsync(
			externalCreatedOn: request.ExternalCreatedOn,
			gameBetId: request.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return info is null
			? null
			: new GameBetCloseInfoResponse(
				Id: info.Id,
				GameBetId: info.GameBetId,
				GameProviderId: info.GameProviderId,
				GameProviderConfigurationId: info.GameProviderConfigurationId,
				GameId: info.GameId,
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				CurrencyCode: info.CurrencyCode,
				WalletId: info.WalletId,
				ExternalRoundId: info.ExternalRoundId,
				ExternalBetId: info.ExternalBetId,
				ExternalCreatedOn: info.ExternalCreatedOn,
				ExternalClosedOn: info.ExternalClosedOn,
				ExternalPayoffOn: info.ExternalPayoffOn,
				ExternalResettleOn: info.ExternalResettleOn,
				BetState: info.BetState,
				BetType: info.BetType,
				FreeGameState: info.FreeGameState,
				GambleState: info.GambleState,
				CashoutState: info.CashoutState,
				ResettleState: info.ResettleState,
				JackpotState: info.JackpotState,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				WinLoss: info.WinLoss,
				Turnover: info.Turnover,
				Commission: info.Commission,
				Contribute: info.Contribute,
				Jackpot: info.Jackpot,
				CreatedOn: info.CreatedOn,
				ClosedOn: info.ClosedOn,
				UpdatedOn: info.UpdatedOn,
				ResetState: info.ResetState,
				ResetOn: info.ResetOn,
				AdditionalData: info.AdditionalData,
				ExtendedData: info.ExtendedData);
	}
}
