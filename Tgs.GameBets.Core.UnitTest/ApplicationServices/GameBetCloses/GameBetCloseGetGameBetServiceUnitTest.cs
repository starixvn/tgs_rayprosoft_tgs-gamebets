using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetCloses;
using Tgs.GameBets.Core.Models.GameBetCloses;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetCloses;

public class GameBetCloseGetGameBetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();

		var id = Guid.NewGuid();
		var gameBetId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var request = new GameBetCloseGetGameBetRequest(
			ExternalCreatedOn: externalCreatedOn,
			GameBetId: gameBetId);

		var gameBetCloseInfo = new GameBetCloseInfoModel(
			Id: id,
			GameBetId: gameBetId,
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: gameId,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Enums.BetState.Win,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: externalCreatedOn,
			ClosedOn: externalClosedOn,
			UpdatedOn: externalClosedOn,
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			closedOn: Arg.Is(request.ExternalCreatedOn),
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetCloseInfoModel?>(gameBetCloseInfo));

		var sut = new GameBetCloseGetGameBetService(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetCloseInfo.Id, actual.Id);
		Assert.Equal(gameBetCloseInfo.GameBetId, actual.GameBetId);
		Assert.Equal(gameBetCloseInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetCloseInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetCloseInfo.GameId, actual.GameId);
		Assert.Equal(gameBetCloseInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetCloseInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetCloseInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetCloseInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetCloseInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetCloseInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetCloseInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetCloseInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetCloseInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetCloseInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetCloseInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetCloseInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetCloseInfo.BetState, actual.BetState);
		Assert.Equal(gameBetCloseInfo.BetType, actual.BetType);
		Assert.Equal(gameBetCloseInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetCloseInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetCloseInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetCloseInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetCloseInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetCloseInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetCloseInfo.Bet, actual.Bet);
		Assert.Equal(gameBetCloseInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetCloseInfo.Settle, actual.Settle);
		Assert.Equal(gameBetCloseInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetCloseInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetCloseInfo.Commission, actual.Commission);
		Assert.Equal(gameBetCloseInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetCloseInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetCloseInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetCloseInfo.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Data_Is_Empty()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();

		var gameBetId = Guid.NewGuid();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var request = new GameBetCloseGetGameBetRequest(
			ExternalCreatedOn: externalCreatedOn,
			GameBetId: gameBetId);

		_ = fakeRepository.GetAsync(
			closedOn: Arg.Is(request.ExternalCreatedOn),
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetCloseInfoModel?>(null));

		var sut = new GameBetCloseGetGameBetService(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Null(actual);
	}
}
