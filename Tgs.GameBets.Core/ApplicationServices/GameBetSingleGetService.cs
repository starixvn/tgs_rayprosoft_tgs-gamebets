using System.Text.Json.Nodes;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Core.ApplicationServices;

internal class GameBetSingleGetService(
	TimeProvider timeProvider,
	IGameBetRepository repository) : IGetService<GameBetSingleQuery, GameBetReslut?>
{
	public async ValueTask<GameBetReslut?> GetAsync(GameBetSingleQuery query, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = query.ExternalCreatedOn ?? timeProvider.GetUtcNow().AddDays(-60);
		var gameBetPrimaryKey = await repository.GetPrimaryKeyAsync(
			id: query.Id,
			externalCreatedOn: externalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (gameBetPrimaryKey is null)
		{
			return null;
		}

		var gameBetInfo = await repository.GetAsync(
			gameProviderId: gameBetPrimaryKey.GameProviderId,
			gameId: gameBetPrimaryKey.GameId,
			walletId: gameBetPrimaryKey.WalletId,
			externalBetId: gameBetPrimaryKey.ExternalBetId,
			externalCreatedOn: gameBetPrimaryKey.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return gameBetInfo is null
			? null
			: new GameBetReslut(
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
