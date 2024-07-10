using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices;
using Tgs.GameBets.Core.Models;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices;

public class GameBetSingleByExternalBetGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameBetRepository>();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var query = new GameBetSingleByExternalBetQuery(
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn);

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
			}.ToJsonString());
		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalBetId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GameBetSingleByExternalBetGetService(
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
		Assert.Equal(gameBetInfo.AdditionalData, actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task Data_Is_Empty()
	{
		var fakeRepository = Substitute.For<IGameBetRepository>();

		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var query = new GameBetSingleByExternalBetQuery(
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn);

		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalBetId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetInfoModel?)null);

		var sut = new GameBetSingleByExternalBetGetService(
			fakeRepository);

		var actual = await sut.GetAsync(query);

		Assert.Null(actual);
	}
}
