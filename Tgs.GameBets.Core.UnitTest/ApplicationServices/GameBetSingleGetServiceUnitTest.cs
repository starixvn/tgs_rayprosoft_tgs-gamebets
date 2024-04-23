using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices;
using Tgs.GameBets.Core.Models;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices;

public class GameBetSingleGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeRepository = Substitute.For<IGameBetRepository>();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var playerCode = "test";
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var query = new GameBetSingleQuery(
			Id: id,
			ExternalCreatedOn: externalCreatedOn);

		var gameBetPrimaryKey = new GameBetPrimaryKeyModel(
			Id: id,
			GameProviderId: gameProviderId,
			GameId: gameId,
			PlayerCode: playerCode,
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn);
		_ = fakeRepository.GetPrimaryKeyAsync(
			id: Arg.Is(query.Id),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetPrimaryKey);

		var gameBetInfo = new GameBetInfoModel(
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
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
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
			});
		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(gameBetPrimaryKey.GameProviderId),
			gameId: Arg.Is(gameBetPrimaryKey.GameId),
			walletId: Arg.Is(gameBetPrimaryKey.WalletId),
			externalBetId: Arg.Is(gameBetPrimaryKey.ExternalBetId),
			externalCreatedOn: Arg.Is(gameBetPrimaryKey.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GameBetSingleGetService(
			fakeTimeProvider,
			fakeRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameBetInfo.Id, actual.Id);
		Assert.Equal(gameBetInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetInfo.GameId, actual.GameId);
		Assert.Equal(gameBetInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetInfo.BetState, actual.BetState);
		Assert.Equal(gameBetInfo.BetType, actual.BetType);
		Assert.Equal(gameBetInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetInfo.Bet, actual.Bet);
		Assert.Equal(gameBetInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetInfo.Settle, actual.Settle);
		Assert.Equal(gameBetInfo.Commission, actual.Commission);
		Assert.Equal(gameBetInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetInfo.AdditionalData.ToJsonString(), actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task Query_ExternalCreatedOn_Is_Null_Set_Default_Time()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeRepository = Substitute.For<IGameBetRepository>();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var playerCode = "test";
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var query = new GameBetSingleQuery(
			Id: id,
			ExternalCreatedOn: null);

		var currentTime = new DateTimeOffset(2024, 04, 23, 13, 45, 50, TimeSpan.FromHours(+8));
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var gameBetPrimaryKey = new GameBetPrimaryKeyModel(
			Id: id,
			GameProviderId: gameProviderId,
			GameId: gameId,
			PlayerCode: playerCode,
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn);
		_ = fakeRepository.GetPrimaryKeyAsync(
			id: Arg.Is(query.Id),
			externalCreatedOn: Arg.Is(currentTime.AddDays(-60)),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetPrimaryKey);

		var gameBetInfo = new GameBetInfoModel(
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
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
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
			});
		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(gameBetPrimaryKey.GameProviderId),
			gameId: Arg.Is(gameBetPrimaryKey.GameId),
			walletId: Arg.Is(gameBetPrimaryKey.WalletId),
			externalBetId: Arg.Is(gameBetPrimaryKey.ExternalBetId),
			externalCreatedOn: Arg.Is(gameBetPrimaryKey.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GameBetSingleGetService(
			fakeTimeProvider,
			fakeRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameBetInfo.Id, actual.Id);
		Assert.Equal(gameBetInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetInfo.GameId, actual.GameId);
		Assert.Equal(gameBetInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetInfo.BetState, actual.BetState);
		Assert.Equal(gameBetInfo.BetType, actual.BetType);
		Assert.Equal(gameBetInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetInfo.Bet, actual.Bet);
		Assert.Equal(gameBetInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetInfo.Settle, actual.Settle);
		Assert.Equal(gameBetInfo.Commission, actual.Commission);
		Assert.Equal(gameBetInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetInfo.AdditionalData.ToJsonString(), actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task GameBet_Primary_Key_Data_Is_Empty()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeRepository = Substitute.For<IGameBetRepository>();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var query = new GameBetSingleQuery(
			Id: id,
			ExternalCreatedOn: externalCreatedOn);

		_ = fakeRepository.GetPrimaryKeyAsync(
			id: Arg.Is(query.Id),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetPrimaryKeyModel?)null);

		var sut = new GameBetSingleGetService(
			fakeTimeProvider,
			fakeRepository);

		var actual = await sut.GetAsync(query);

		Assert.Null(actual);

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			gameId: Arg.Is(gameId),
			walletId: Arg.Is(walletId),
			externalBetId: Arg.Is(externalBetId),
			externalCreatedOn: Arg.Is(externalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBetInfo_Data_Is_Empty()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeRepository = Substitute.For<IGameBetRepository>();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var playerCode = "test";
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var query = new GameBetSingleQuery(
			Id: id,
			ExternalCreatedOn: externalCreatedOn);

		var gameBetPrimaryKey = new GameBetPrimaryKeyModel(
			Id: id,
			GameProviderId: gameProviderId,
			GameId: gameId,
			PlayerCode: playerCode,
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn);
		_ = fakeRepository.GetPrimaryKeyAsync(
			id: Arg.Is(query.Id),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetPrimaryKey);

		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(gameBetPrimaryKey.GameProviderId),
			gameId: Arg.Is(gameBetPrimaryKey.GameId),
			walletId: Arg.Is(gameBetPrimaryKey.WalletId),
			externalBetId: Arg.Is(gameBetPrimaryKey.ExternalBetId),
			externalCreatedOn: Arg.Is(gameBetPrimaryKey.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetInfoModel?)null);

		var sut = new GameBetSingleGetService(
			fakeTimeProvider,
			fakeRepository);

		var actual = await sut.GetAsync(query);

		Assert.Null(actual);
	}
}
