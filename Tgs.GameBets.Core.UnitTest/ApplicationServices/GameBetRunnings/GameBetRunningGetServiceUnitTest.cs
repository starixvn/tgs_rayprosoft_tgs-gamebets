using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;
using Tgs.GameBets.Core.Models.GameBetRunnings;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetRunnings;

public class GameBetRunningGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var gameBetId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderConfigurationId = Guid.NewGuid();
		var resellerId = Guid.NewGuid();
		var licenseeId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var playerCode = "test";
		var currencyCode = "CNY";
		var externalRoundId = Guid.NewGuid().ToString("N");

		var request = new GameBetRunningGetRequest(
			GameBetId: gameBetId);

		var gameBetRunningInfo = new GameBetRunningInfoModel(
			GameBetId: gameBetId,
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: gameProviderConfigurationId,
			GameId: gameId,
			ResellerId: resellerId,
			LicenseeId: licenseeId,
			BrandId: brandId,
			PlayerCode: playerCode,
			CurrencyCode: currencyCode,
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			CreatedOn: externalCreatedOn,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			gameBetId: Arg.Is(gameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetRunningInfoModel?>(gameBetRunningInfo));

		var sut = new GameBetRunningGetService(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetRunningInfo.GameBetId, actual.GameBetId);
		Assert.Equal(gameBetRunningInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetRunningInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetRunningInfo.GameId, actual.GameId);
		Assert.Equal(gameBetRunningInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetRunningInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetRunningInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetRunningInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetRunningInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetRunningInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetRunningInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetRunningInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetRunningInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetRunningInfo.BetType, actual.BetType);
		Assert.Equal(gameBetRunningInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetRunningInfo.Bet, actual.Bet);
		Assert.Equal(gameBetRunningInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetRunningInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetRunningInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBetRunningInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetRunningInfo.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Data_Is_Empty()
	{
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var gameBetId = Guid.NewGuid();

		var request = new GameBetRunningGetRequest(
			GameBetId: gameBetId);

		_ = fakeRepository.GetAsync(
			gameBetId: Arg.Is(gameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetRunningInfoModel?>(null));

		var sut = new GameBetRunningGetService(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Null(actual);
	}
}
