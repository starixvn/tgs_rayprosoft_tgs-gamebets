using System.Text.Json.Nodes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;

namespace Tgs.GameBets.IntegrationUnitTest;
public class GameBetsClientGameBetsRapidTestUnitTest
{

	[Fact]
	public async Task GameBetRapidTest_Command()
	{
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new Common.ViewModels.GameBetRapidTestCommandViewModel
		{
			GameProviderCode = "KY",
			SubGameProviderCode = "KY",
			GameId = 1000017,
			ResellerCode = "TGS",
			LicenseeCode = "TGS",
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			CurrencyCode = "CNY",
			WalletId = Guid.NewGuid(),
			ExternalRoundId = "50-1691469296-370404138-1",
			ExternalBetId = "50-1691469296-370404138-1",
			ExternalGameId = "910",
			BetState = Common.Enums.BetState.Win,
			BetType = Common.Enums.BetType.Normal,
			FreeGameState = Common.Enums.FreeGameState.None,
			GambleState = Common.Enums.GambleState.None,
			CashoutState = Common.Enums.CashoutState.None,
			ResettleState = Common.Enums.ResettleState.None,
			JackpotState = Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 4.0m,
			ValidBet = 4.0m,
			Settle = 8.0m,
			WinLoss = 4.0m,
			Turnover = 4.0m,
			Commission = 0.0m,
			Contribute = 0.0m,
			Jackpot = 0.0m
		};

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();
			_ = fakeMediator.Send(
				request: Arg.Is<GameBetRapidTestCommandRequest>(compare =>
					compare.GameProviderCode == source.GameProviderCode &&
					compare.SubGameProviderCode == source.SubGameProviderCode &&
					compare.GameId == source.GameId &&
					compare.ResellerCode == source.ResellerCode &&
					compare.LicenseeCode == source.LicenseeCode &&
					compare.BrandCode == source.BrandCode &&
					compare.PlayerCode == source.PlayerCode &&
					compare.Username == source.Username &&
					compare.CurrencyCode == source.CurrencyCode &&
					compare.WalletId == source.WalletId &&
					compare.ExternalRoundId == source.ExternalRoundId &&
					compare.ExternalBetId == source.ExternalBetId &&
					compare.ExternalGameId == source.ExternalGameId &&
					compare.BetState == source.BetState &&
					compare.BetType == source.BetType &&
					compare.FreeGameState == source.FreeGameState &&
					compare.GambleState == source.GambleState &&
					compare.CashoutState == source.CashoutState &&
					compare.ResettleState == source.ResettleState &&
					compare.JackpotState == source.JackpotState &&
					compare.CurrencyRatio == source.CurrencyRatio &&
					compare.Bet == source.Bet &&
					compare.ValidBet == source.ValidBet &&
					compare.Settle == source.Settle &&
					compare.WinLoss == source.WinLoss &&
					compare.Turnover == source.Turnover &&
					compare.Commission == source.Commission &&
					compare.Contribute == source.Contribute &&
					compare.Jackpot == source.Jackpot),
				cancellationToken: Arg.Any<CancellationToken>()
			)
			.Returns(true);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetRapidTestCommandAsync(source);

		Assert.True(actual);
	}

	[Fact]
	public async Task GameBetRapidTest_Expire()
	{
		var gameProviderCode = "KY";

		var fakeMediator = Substitute.For<IMediator>();

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeMediator)));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		await client.GameBetRapidTestExpireAsync(gameProviderCode);

		_ = fakeMediator
			.Received(1)
			.Send(
			request: Arg.Is(new GameBetRapidTestExpireCommandRequest(
				GameProviderCode: gameProviderCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBetRapidTest_Initialization()
	{
		var gameProviderCode = "KY";

		var fakeMediator = Substitute.For<IMediator>();

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeMediator)));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		await client.GameBetRapidTestInitializationAsync(gameProviderCode);

		_ = fakeMediator
			.Received(1)
			.Send(
			request: Arg.Is(new GameBetRapidTestInitializationCommandRequest(
				GameProviderCode: gameProviderCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
