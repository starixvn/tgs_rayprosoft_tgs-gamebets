using MediatR;
using Tgs.GameBets.Core.Models.GameBetRapidTests;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;

internal class GameBetRapidTestCommandService(
	IGameBetRapidTestRepository repository) : IRequestHandler<GameBetRapidTestCommandRequest, bool>
{
	public Task<bool> Handle(GameBetRapidTestCommandRequest request, CancellationToken cancellationToken)
	=> repository.MergeAsync(
		source: new GameBetRapidTestMerge(
			GameProviderCode: request.GameProviderCode,
			SubGameProviderCode: request.SubGameProviderCode,
			GameId: request.GameId,
			ResellerCode: request.ResellerCode,
			LicenseeCode: request.LicenseeCode,
			BrandCode: request.BrandCode,
			PlayerCode: request.PlayerCode,
			Username: request.Username,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalGameId: request.ExternalGameId,
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
			Jackpot: request.Jackpot),
			cancellationToken: cancellationToken).AsTask();
}
