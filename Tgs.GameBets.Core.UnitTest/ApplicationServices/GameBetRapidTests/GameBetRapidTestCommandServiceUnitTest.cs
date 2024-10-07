using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;
using Tgs.GameBets.Core.Models.GameBetRapidTests;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetRapidTests;

public class GameBetRapidTestCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRapidTestRepository = Substitute.For<IGameBetRapidTestRepository>();

		var request = new GameBetRapidTestCommandRequest(
			GameProviderCode: "KY",
			SubGameProviderCode: "KY",
			GameId: 1000017,
			ResellerCode: "TGS",
			LicenseeCode: "TGS",
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "50-1691469296-370404138-1",
			ExternalBetId: "50-1691469296-370404138-1",
			ExternalGameId: "910",
			BetState: Common.Enums.BetState.Win,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 4.0m,
			ValidBet: 4.0m,
			Settle: 8.0m,
			WinLoss: 4.0m,
			Turnover: 4.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);

		_ = fakeGameBetRapidTestRepository.MergeAsync(
			source: Arg.Is<GameBetRapidTestMerge>(compare =>
				compare.GameProviderCode == request.GameProviderCode &&
				compare.SubGameProviderCode == request.SubGameProviderCode &&
				compare.GameId == request.GameId &&
				compare.ResellerCode == request.ResellerCode &&
				compare.LicenseeCode == request.LicenseeCode &&
				compare.BrandCode == request.BrandCode &&
				compare.PlayerCode == request.PlayerCode &&
				compare.Username == request.Username &&
				compare.CurrencyCode == request.CurrencyCode &&
				compare.WalletId == request.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.BetState == request.BetState &&
				compare.BetType == request.BetType &&
				compare.FreeGameState == request.FreeGameState &&
				compare.GambleState == request.GambleState &&
				compare.CashoutState == request.CashoutState &&
				compare.ResettleState == request.ResettleState &&
				compare.JackpotState == request.JackpotState &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.Settle == request.Settle &&
				compare.WinLoss == request.WinLoss &&
				compare.Turnover == request.Turnover &&
				compare.Commission == request.Commission &&
				compare.Contribute == request.Contribute &&
				compare.Jackpot == request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
		.Returns(true);

		var sut = new GameBetRapidTestCommandService(
			fakeGameBetRapidTestRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.True(actual);
	}
}
