using System.Text.Json.Nodes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Core.ApplicationServices.GameBets;

namespace Tgs.GameBets.IntegrationUnitTest;

public class GameBetsClientGameBetUnitTest
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

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();
			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetCommandRequest(
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
					AdditionalData: source.AdditionalData.ToJsonString())),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Common.Enums.GameBetCommandResult.Add);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetCommandAsync(source);

		Assert.Equal(Common.Enums.GameBetCommandResult.Add, actual);
	}

	[Fact]
	public async Task GameBet_Get_Id_ExternalCreatedOn()
	{
		var id = Guid.NewGuid();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var response = new GameBetInfoResponse(
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
			ExternalRoundId: Guid.NewGuid().ToString(),
			ExternalBetId: externalCreatedOn.ToUnixTimeMilliseconds().ToString(),
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
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
			Jackpot: 0.0m,
			CreatedOn: externalCreatedOn,
			ClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalCreatedOn.ToUnixTimeMilliseconds().ToString()
			});

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetGetIdRequest(
					ExternalCreatedOn: externalCreatedOn,
					Id: id)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(externalCreatedOn, id);

		Assert.NotNull(actual);
		Assert.Equal(response.Id, actual.Id);
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
		Assert.Equal(response.Commission, actual.Commission);
		Assert.Equal(response.Turnover, actual.Turnover);
		Assert.Equal(response.WinLoss, actual.WinLoss);
		Assert.Equal(response.Jackpot, actual.Jackpot);
		Assert.Equal(response.AdditionalData.ToJsonString(), actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task GameBet_Get_Id_ExternalCreatedOn_Data_Is_Empty()
	{
		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));
		var externalPayoffOn = new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8));

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetGetIdRequest(
					ExternalCreatedOn: externalCreatedOn,
					Id: id)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetInfoResponse?>(null));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(externalCreatedOn, id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task GameBet_Get_External()
	{
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var response = new GameBetInfoResponse(
			Id: Guid.NewGuid(),
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: gameId,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: Guid.NewGuid().ToString(),
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
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
			Jackpot: 0.0m,
			CreatedOn: externalCreatedOn,
			ClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalBetId
			});

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetGetExternalRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetInfoResponse?>(response));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(
			externalCreatedOn: externalCreatedOn,
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId);

		Assert.NotNull(actual);
		Assert.Equal(response.Id, actual.Id);
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
		Assert.Equal(response.Commission, actual.Commission);
		Assert.Equal(response.Turnover, actual.Turnover);
		Assert.Equal(response.WinLoss, actual.WinLoss);
		Assert.Equal(response.Jackpot, actual.Jackpot);
		Assert.Equal(response.AdditionalData.ToJsonString(), actual.AdditionalData.ToJsonString());
	}

	[Fact]
	public async Task GameBet_Get_External_Data_Is_Empty()
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
				request: Arg.Is(new GameBetGetExternalRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetInfoResponse?>(null));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(
			externalCreatedOn: externalCreatedOn,
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId);

		Assert.Null(actual);
	}
}
