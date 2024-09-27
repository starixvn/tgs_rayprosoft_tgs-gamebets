using System.Text.Json.Nodes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Core.ApplicationServices.GameBetCloses;

namespace Tgs.GameBets.IntegrationUnitTest;
public class GameBetsClientGameBetCloses
{
	[Fact]
	public async Task GameBetCloses_Command()
	{
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new Common.ViewModels.GameBetCloseCommandViewModel
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
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalPayoffOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = Common.Enums.BetState.Win,
			BetType = Common.Enums.BetType.Normal,
			FreeGameState = Common.Enums.FreeGameState.None,
			GambleState = Common.Enums.GambleState.None,
			CashoutState = Common.Enums.CashoutState.None,
			ResettleState = Common.Enums.ResettleState.None,
			JackpotState = Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 100.0m,
			ValidBet = 50.0m,
			Settle = 97.5m,
			WinLoss = 47.5m,
			Turnover = 50.0m,
			Commission = 0.1m,
			Jackpot = 0.01m,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData = new JsonObject { }
		};

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();
			_ = fakeMediator.Send(
				request: Arg.Is<GameBetCloseCommandRequest>(compare =>
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
					compare.ExternalClosedOn == source.ExternalClosedOn &&
					compare.ExternalPayoffOn == source.ExternalPayoffOn &&
					compare.ExternalResettleOn == source.ExternalResettleOn &&
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
					compare.Jackpot == source.Jackpot &&
					compare.AdditionalData != null &&
					compare.ExtendedData != null),
				cancellationToken: Arg.Any<CancellationToken>()
			)
			.Returns(Common.Enums.GameBetCommandResult.Add);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetClosesCommandAsync(source);

		Assert.Equal(Common.Enums.GameBetCommandResult.Add, actual);
	}

	[Fact]
	public async Task GameBetCloses_Get_Id_ExternalCreatedOn()
	{
		var gameBetId = Guid.NewGuid();

		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = externalCreatedOn.ToUnixTimeMilliseconds().ToString();

		var response = new GameBetCloseInfoResponse(
			Id: Guid.NewGuid(),
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
			ExternalClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
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
			ClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
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
				request: Arg.Is(new GameBetCloseGetGameBetRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameBetId: gameBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetClosesGetAsync(externalCreatedOn, gameBetId);

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
		Assert.Equal(response.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(response.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(response.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(response.BetState, actual.BetState);
		Assert.Equal(response.BetType, actual.BetType);
		Assert.Equal(response.FreeGameState, actual.FreeGameState);
		Assert.Equal(response.GambleState, actual.GambleState);
		Assert.Equal(response.CashoutState, actual.CashoutState);
		Assert.Equal(response.ResettleState, actual.ResettleState);
		Assert.Equal(response.JackpotState, actual.JackpotState);
		Assert.Equal(response.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(response.Bet, actual.Bet);
		Assert.Equal(response.ValidBet, actual.ValidBet);
		Assert.Equal(response.Settle, actual.Settle);
		Assert.Equal(response.WinLoss, actual.WinLoss);
		Assert.Equal(response.Turnover, actual.Turnover);
		Assert.Equal(response.Commission, actual.Commission);
		Assert.Equal(response.Contribute, actual.Contribute);
		Assert.Equal(response.Jackpot, actual.Jackpot);
		Assert.Equal(response.AdditionalData, actual.AdditionalData.ToJsonString());
		Assert.Equal(response.ExtendedData, actual.ExtendedData.ToJsonString());
	}

	[Fact]
	public async Task GameBetCloses_Get_Id_ExternalCreatedOn_Data_Is_Empty()
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
				request: Arg.Is(new GameBetCloseGetGameBetRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameBetId: gameBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetCloseInfoResponse?>(null));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetClosesGetAsync(externalCreatedOn, gameBetId);

		Assert.Null(actual);
	}

	[Fact]
	public async Task GameBetCloses_Get_External()
	{
		var gameBetId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalBetId = externalCreatedOn.ToUnixTimeMilliseconds().ToString();

		var response = new GameBetCloseInfoResponse(
			Id: Guid.NewGuid(),
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
			ExternalClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
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
			ClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
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
				request: Arg.Is(new GameBetCloseGetExternalRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetCloseInfoResponse?>(response));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetClosesGetAsync(
			externalCreatedOn: externalCreatedOn,
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId);

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
		Assert.Equal(response.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(response.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(response.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(response.BetState, actual.BetState);
		Assert.Equal(response.BetType, actual.BetType);
		Assert.Equal(response.FreeGameState, actual.FreeGameState);
		Assert.Equal(response.GambleState, actual.GambleState);
		Assert.Equal(response.CashoutState, actual.CashoutState);
		Assert.Equal(response.ResettleState, actual.ResettleState);
		Assert.Equal(response.JackpotState, actual.JackpotState);
		Assert.Equal(response.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(response.Bet, actual.Bet);
		Assert.Equal(response.ValidBet, actual.ValidBet);
		Assert.Equal(response.Settle, actual.Settle);
		Assert.Equal(response.WinLoss, actual.WinLoss);
		Assert.Equal(response.Turnover, actual.Turnover);
		Assert.Equal(response.Commission, actual.Commission);
		Assert.Equal(response.Contribute, actual.Contribute);
		Assert.Equal(response.Jackpot, actual.Jackpot);
		Assert.Equal(response.AdditionalData, actual.AdditionalData.ToJsonString());
		Assert.Equal(response.ExtendedData, actual.ExtendedData.ToJsonString());
	}

	[Fact]
	public async Task GameBetCloses_Get_External_Data_Is_Empty()
	{
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetCloseGetExternalRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetCloseInfoResponse?>(null));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetClosesGetAsync(
			externalCreatedOn: externalCreatedOn,
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId);

		Assert.Null(actual);
	}
}
