using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.IntegrationUnitTest;

public class GameBetUnitTest
{
	[Fact]
	public async Task GameBet_Command()
	{
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new Common.ViewModels.GameBetCommandViewModel(
			Id: Guid.NewGuid(),
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
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var web = new GameBetsApplicationTest((Action<Microsoft.AspNetCore.Hosting.IWebHostBuilder>?)(builder => _ = builder.ConfigureServices((Action<IServiceCollection>)(services =>
		{
			var fakeService = Substitute.For<ICommandService<GameBetCommand>>();
			_ = fakeService.ExecuteAsync(
				command: Arg.Is(new GameBetCommand(
					GameBetId: source.Id,
					GameProviderId: source.GameProviderId,
					GameProviderConfigurationId: source.GameProviderConfigurationId,
					GameId: source.GameId,
					ResellerId: source.ResellerId,
					LicenseeId: source.LicenseeId,
					BrandId: source.BrandId,
					PlayerCode: source.PlayerCode,
					CurrencyCode: source.CurrencyCode,
					WalletId: source.WalletId,
					ExternalRoundId: source.ExternalRoundId,
					ExternalBetId: source.ExternalBetId,
					ExternalCreatedOn: source.ExternalCreatedOn,
					ExternalClosedOn: source.ExternalClosedOn,
					ExternalPayoffOn: source.ExternalPayoffOn,
					ExternalResettleOn: source.ExternalResettleOn,
					BetState: source.BetState,
					BetType: source.BetType,
					FreeGameState: source.FreeGameState,
					GambleState: source.GambleState,
					CashoutState: source.CashoutState,
					ResettleState: source.ResettleState,
					JackpotState: source.JackpotState,
					CurrencyRatio: source.CurrencyRatio,
					Bet: source.Bet,
					ValidBet: source.ValidBet,
					Settle: source.Settle,
					Commission: source.Commission,
					Turnover: source.Turnover,
					WinLoss: source.WinLoss,
					Jackpot: source.Jackpot,
					AdditionalData: source.AdditionalData)),
				cancellationToken: Arg.Any<CancellationToken>());

			_ = services.AddTransient<ICommandService<GameBetCommand>>(_ => fakeService);
		}))));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		await client.GameBetCommandAsync(source);
	}

	[Fact]
	public async Task GameBet_Get()
	{
		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var reslut = new GameBetReslut(
			Id: id,
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

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<GameBetSingleQuery, GameBetReslut?>>();

			_ = fakeService.GetAsync(
				query: Arg.Is(new GameBetSingleQuery(
					Id: id,
					ExternalCreatedOn: null)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reslut);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(id);

		Assert.NotNull(actual);
		Assert.Equal(reslut.Id, actual.Id);
		Assert.Equal(reslut.GameProviderId, actual.GameProviderId);
		Assert.Equal(reslut.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(reslut.GameId, actual.GameId);
		Assert.Equal(reslut.ResellerId, actual.ResellerId);
		Assert.Equal(reslut.LicenseeId, actual.LicenseeId);
		Assert.Equal(reslut.BrandId, actual.BrandId);
		Assert.Equal(reslut.PlayerCode, actual.PlayerCode);
		Assert.Equal(reslut.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(reslut.WalletId, actual.WalletId);
		Assert.Equal(reslut.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(reslut.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(reslut.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(reslut.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(reslut.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(reslut.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(reslut.BetState, actual.BetState);
		Assert.Equal(reslut.BetType, actual.BetType);
		Assert.Equal(reslut.FreeGameState, actual.FreeGameState);
		Assert.Equal(reslut.GambleState, actual.GambleState);
		Assert.Equal(reslut.CashoutState, actual.CashoutState);
		Assert.Equal(reslut.ResettleState, actual.ResettleState);
		Assert.Equal(reslut.JackpotState, actual.JackpotState);
		Assert.Equal(reslut.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(reslut.Bet, actual.Bet);
		Assert.Equal(reslut.ValidBet, actual.ValidBet);
		Assert.Equal(reslut.Settle, actual.Settle);
		Assert.Equal(reslut.Commission, actual.Commission);
		Assert.Equal(reslut.Turnover, actual.Turnover);
		Assert.Equal(reslut.WinLoss, actual.WinLoss);
		Assert.Equal(reslut.Jackpot, actual.Jackpot);
		Assert.Equal(reslut.AdditionalData.ToJsonString(), actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task GameBet_Get_Data_Is_Empty()
	{
		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<GameBetSingleQuery, GameBetReslut?>>();

			_ = fakeService.GetAsync(
				query: Arg.Is(new GameBetSingleQuery(
					Id: id,
					ExternalCreatedOn: null)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetReslut?)null);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task GameBet_Get_ExternalCreatedOn()
	{
		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var reslut = new GameBetReslut(
			Id: id,
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

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<GameBetSingleQuery, GameBetReslut?>>();

			_ = fakeService.GetAsync(
				query: Arg.Is(new GameBetSingleQuery(
					Id: id,
					ExternalCreatedOn: externalCreatedOn)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reslut);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(id, externalCreatedOn);

		Assert.NotNull(actual);
		Assert.Equal(reslut.Id, actual.Id);
		Assert.Equal(reslut.GameProviderId, actual.GameProviderId);
		Assert.Equal(reslut.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(reslut.GameId, actual.GameId);
		Assert.Equal(reslut.ResellerId, actual.ResellerId);
		Assert.Equal(reslut.LicenseeId, actual.LicenseeId);
		Assert.Equal(reslut.BrandId, actual.BrandId);
		Assert.Equal(reslut.PlayerCode, actual.PlayerCode);
		Assert.Equal(reslut.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(reslut.WalletId, actual.WalletId);
		Assert.Equal(reslut.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(reslut.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(reslut.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(reslut.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(reslut.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(reslut.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(reslut.BetState, actual.BetState);
		Assert.Equal(reslut.BetType, actual.BetType);
		Assert.Equal(reslut.FreeGameState, actual.FreeGameState);
		Assert.Equal(reslut.GambleState, actual.GambleState);
		Assert.Equal(reslut.CashoutState, actual.CashoutState);
		Assert.Equal(reslut.ResettleState, actual.ResettleState);
		Assert.Equal(reslut.JackpotState, actual.JackpotState);
		Assert.Equal(reslut.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(reslut.Bet, actual.Bet);
		Assert.Equal(reslut.ValidBet, actual.ValidBet);
		Assert.Equal(reslut.Settle, actual.Settle);
		Assert.Equal(reslut.Commission, actual.Commission);
		Assert.Equal(reslut.Turnover, actual.Turnover);
		Assert.Equal(reslut.WinLoss, actual.WinLoss);
		Assert.Equal(reslut.Jackpot, actual.Jackpot);
		Assert.Equal(reslut.AdditionalData.ToJsonString(), actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task GameBet_Get_ExternalCreatedOn_Data_Is_Empty()
	{
		var gameBetId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<GameBetSingleQuery, GameBetReslut?>>();

			_ = fakeService.GetAsync(
				query: Arg.Is(new GameBetSingleQuery(
					Id: gameBetId,
					ExternalCreatedOn: externalCreatedOn)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetReslut?)null);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(gameBetId, externalCreatedOn);

		Assert.Null(actual);
	}

	[Fact]
	public async Task GameBet_Get_ByExternalBet()
	{
		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var reslut = new GameBetReslut(
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

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IGetService<GameBetSingleByExternalBetQuery, GameBetReslut?>>();

			_ = fakeRepository.GetAsync(
				query: Arg.Is(new GameBetSingleByExternalBetQuery(
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId,
					ExternalCreatedOn: externalCreatedOn)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reslut);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetByExternalBetAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId,
			externalCreatedOn: externalCreatedOn);

		Assert.NotNull(actual);
		Assert.Equal(reslut.Id, actual.Id);
		Assert.Equal(reslut.GameProviderId, actual.GameProviderId);
		Assert.Equal(reslut.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(reslut.GameId, actual.GameId);
		Assert.Equal(reslut.ResellerId, actual.ResellerId);
		Assert.Equal(reslut.LicenseeId, actual.LicenseeId);
		Assert.Equal(reslut.BrandId, actual.BrandId);
		Assert.Equal(reslut.PlayerCode, actual.PlayerCode);
		Assert.Equal(reslut.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(reslut.WalletId, actual.WalletId);
		Assert.Equal(reslut.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(reslut.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(reslut.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(reslut.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(reslut.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(reslut.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(reslut.BetState, actual.BetState);
		Assert.Equal(reslut.BetType, actual.BetType);
		Assert.Equal(reslut.FreeGameState, actual.FreeGameState);
		Assert.Equal(reslut.GambleState, actual.GambleState);
		Assert.Equal(reslut.CashoutState, actual.CashoutState);
		Assert.Equal(reslut.ResettleState, actual.ResettleState);
		Assert.Equal(reslut.JackpotState, actual.JackpotState);
		Assert.Equal(reslut.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(reslut.Bet, actual.Bet);
		Assert.Equal(reslut.ValidBet, actual.ValidBet);
		Assert.Equal(reslut.Settle, actual.Settle);
		Assert.Equal(reslut.Commission, actual.Commission);
		Assert.Equal(reslut.Turnover, actual.Turnover);
		Assert.Equal(reslut.WinLoss, actual.WinLoss);
		Assert.Equal(reslut.Jackpot, actual.Jackpot);
		Assert.Equal(reslut.AdditionalData.ToJsonString(), actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task GameBet_Get_ByExternalBet_Data_Is_Empty()
	{
		var gameBetId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IGetService<GameBetSingleByExternalBetQuery, GameBetReslut?>>();

			_ = fakeRepository.GetAsync(
				query: Arg.Is(new GameBetSingleByExternalBetQuery(
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId,
					ExternalCreatedOn: externalCreatedOn)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetReslut?)null);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetByExternalBetAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId,
			externalCreatedOn: externalCreatedOn);

		Assert.Null(actual);
	}
}
