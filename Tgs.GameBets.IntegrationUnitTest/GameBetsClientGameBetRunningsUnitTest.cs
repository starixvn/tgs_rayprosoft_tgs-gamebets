using System.Text.Json.Nodes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;

namespace Tgs.GameBets.IntegrationUnitTest;

public class GameBetsClientGameBetRunningsUnitTest
{
	[Fact]
	public async Task GameBetRunnings_Command()
	{
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new Common.ViewModels.GameBetRunningCommandViewModel
		{
			GameBetId = Guid.NewGuid(),
			GameProviderId = Guid.NewGuid(),
			GameProviderConfigurationId = Guid.NewGuid(),
			GameId = 1,
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			WalletId = Guid.NewGuid(),
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn,
			BetType = Common.Enums.BetType.Normal,
			CurrencyRatio = 1.0m,
			Bet = 100.0m,
			ValidBet = 50.0m,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData = new JsonObject { }
		};

		var fakeMediator = Substitute.For<IMediator>();
		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeMediator)));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		await client.GameBetRunningCommandAsync(source);

		_ = fakeMediator
			.Received(1)
			.Send(
			request: Arg.Is<GameBetRunningCommandRequest>(compare =>
				compare.GameBetId == source.GameBetId &&
				compare.GameProviderId == source.GameProviderId &&
				compare.GameProviderConfigurationId == source.GameProviderConfigurationId &&
				compare.GameId == source.GameId &&
				compare.ResellerId == source.ResellerId &&
				compare.LicenseeId == source.LicenseeId &&
				compare.BrandId == source.BrandId &&
				compare.PlayerCode == source.PlayerCode &&
				compare.CurrencyCode == source.CurrencyCode &&
				compare.WalletId == source.WalletId &&
				compare.ExternalRoundId == source.ExternalRoundId &&
				compare.ExternalBetId == source.ExternalBetId &&
				compare.ExternalCreatedOn == source.ExternalCreatedOn &&
				compare.BetType == source.BetType &&
				compare.CurrencyRatio == source.CurrencyRatio &&
				compare.Bet == source.Bet &&
				compare.ValidBet == source.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBetRunning_Delete()
	{
		var gameBetId = Guid.NewGuid();

		var fakeMediator = Substitute.For<IMediator>();
		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeMediator)));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		await client.GameBetRunningDeleteAsync(gameBetId);

		_ = fakeMediator
			.Received(1)
			.Send(
			request: Arg.Is<GameBetRunningDeleteRequest>(compare =>
				compare.GameBetId == gameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBetRunnings_Get_Id_ExternalCreatedOn()
	{
		var gameBetId = Guid.NewGuid();

		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = externalCreatedOn.ToUnixTimeMilliseconds().ToString();

		var response = new GameBetRunningInfoResponse(
			GameBetId: gameBetId,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			CreatedOn: externalCreatedOn,
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetRunningGetRequest(
					GameBetId: gameBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetRunningGetAsync(gameBetId);

		Assert.NotNull(actual);
		Assert.Equal(gameBetId, actual.Id);
		Assert.Equal(response.GameProviderId, actual.GameProviderId);
		Assert.Equal(response.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(response.GameId, actual.GameId);
		Assert.Equal(response.ResellerId, actual.ResellerId);
		Assert.Equal(response.LicenseeId, actual.LicenseeId);
		Assert.Equal(response.BrandId, actual.BrandId);
		Assert.Equal(response.PlayerCode, actual.PlayerCode);
		Assert.Equal(response.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(response.WalletId, actual.WalletId);
		Assert.Equal(response.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(response.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(response.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(response.BetType, actual.BetType);
		Assert.Equal(response.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(response.Bet, actual.Bet);
		Assert.Equal(response.ValidBet, actual.ValidBet);
		Assert.Equal(response.AdditionalData, actual.AdditionalData.ToJsonString());
		Assert.Equal(response.ExtendedData, actual.ExtendedData.ToJsonString());
	}

	[Fact]
	public async Task GameBetRunnings_Get_Id_ExternalCreatedOn_Data_Is_Empty()
	{
		var gameBetId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetRunningGetRequest(
					GameBetId: gameBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetRunningInfoResponse?>(null));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetClosesGetAsync(externalCreatedOn, gameBetId);

		Assert.Null(actual);
	}
}
