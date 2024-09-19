using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetMajors;

internal class GameBetMajorGetIdService(
	IGameBetMajorRepository repository) : IRequestHandler<GameBetMajorGetIdRequest, GameBetMajorInfoResponse?>
{
	public async Task<GameBetMajorInfoResponse?> Handle(GameBetMajorGetIdRequest request, CancellationToken cancellationToken)
	{
		var gameBetMajorInfo = await repository.GetAsync(
			externalCreatedOn: request.ExternalCreatedOn,
			id: request.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return gameBetMajorInfo is null
			? null
			: new GameBetMajorInfoResponse(
				Id: gameBetMajorInfo.Id,
				GameProviderId: gameBetMajorInfo.GameProviderId,
				GameProviderConfigurationId: gameBetMajorInfo.GameProviderConfigurationId,
				GameId: gameBetMajorInfo.GameId,
				ResellerId: gameBetMajorInfo.ResellerId,
				LicenseeId: gameBetMajorInfo.LicenseeId,
				BrandId: gameBetMajorInfo.BrandId,
				PlayerCode: gameBetMajorInfo.PlayerCode,
				CurrencyCode: gameBetMajorInfo.CurrencyCode,
				WalletId: gameBetMajorInfo.WalletId,
				ExternalRoundId: gameBetMajorInfo.ExternalRoundId,
				ExternalBetId: gameBetMajorInfo.ExternalBetId,
				ExternalCreatedOn: gameBetMajorInfo.ExternalCreatedOn,
				ExternalClosedOn: gameBetMajorInfo.ExternalClosedOn,
				ExternalPayoffOn: gameBetMajorInfo.ExternalPayoffOn,
				ExternalResettleOn: gameBetMajorInfo.ExternalResettleOn,
				BetState: gameBetMajorInfo.BetState,
				BetType: gameBetMajorInfo.BetType,
				FreeGameState: gameBetMajorInfo.FreeGameState,
				GambleState: gameBetMajorInfo.GambleState,
				CashoutState: gameBetMajorInfo.CashoutState,
				ResettleState: gameBetMajorInfo.ResettleState,
				JackpotState: gameBetMajorInfo.JackpotState,
				CurrencyRatio: gameBetMajorInfo.CurrencyRatio,
				Bet: gameBetMajorInfo.Bet,
				ValidBet: gameBetMajorInfo.ValidBet,
				Settle: gameBetMajorInfo.Settle,
				WinLoss: gameBetMajorInfo.WinLoss,
				Turnover: gameBetMajorInfo.Turnover,
				Commission: gameBetMajorInfo.Commission,
				Contribute: gameBetMajorInfo.Contribute,
				Jackpot: gameBetMajorInfo.Jackpot,
				CreatedOn: gameBetMajorInfo.CreatedOn,
				ClosedOn: gameBetMajorInfo.ClosedOn,
				UpdatedOn: gameBetMajorInfo.UpdatedOn,
				AdditionalData: gameBetMajorInfo.AdditionalData,
				ExtendedData: gameBetMajorInfo.ExtendedData);
	}
}
