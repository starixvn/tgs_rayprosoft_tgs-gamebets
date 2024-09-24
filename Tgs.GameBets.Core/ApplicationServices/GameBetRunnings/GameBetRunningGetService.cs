using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;

internal class GameBetRunningGetService(
	IGameBetRunningRepository repository) : IRequestHandler<GameBetRunningGetRequest, GameBetRunningInfoResponse?>
{
	public async Task<GameBetRunningInfoResponse?> Handle(GameBetRunningGetRequest request, CancellationToken cancellationToken)
	{
		var info = await repository.GetAsync(
			gameBetId: request.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return info is null
			? null
			: new GameBetRunningInfoResponse(
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
				BetType: info.BetType,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				CreatedOn: info.CreatedOn,
				UpdatedOn: info.UpdatedOn,
				AdditionalData: info.AdditionalData,
				ExtendedData: info.ExtendedData);
	}
}
