using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetMajors;
using Tgs.GameBets.Core.Models.GameBetMajors;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetMajors;

public class GameBetMajorGetExternalServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var request = new GameBetMajorGetExternalRequest(
			ExternalCreatedOn: externalCreatedOn,
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalBetId: externalBetId);

		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
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
			CurrencyRatio: 1.0m,
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
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorGetExternalService(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Data_Is_Empty()
	{
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var request = new GameBetMajorGetExternalRequest(
			ExternalCreatedOn: externalCreatedOn,
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalBetId: externalBetId);

		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetMajorInfoModel?>(null));

		var sut = new GameBetMajorGetExternalService(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Null(actual);
	}
}
