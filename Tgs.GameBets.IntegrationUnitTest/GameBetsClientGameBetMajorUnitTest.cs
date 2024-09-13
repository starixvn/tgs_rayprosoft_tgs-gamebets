using System.Text.Json.Nodes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Core.ApplicationServices.GameBetMajors;
using Tgs.GameBets.Core.ApplicationServices.GameBets;

namespace Tgs.GameBets.IntegrationUnitTest;

public class GameBetsClientGameBetMajorUnitTest
{
	[Fact]
	public async Task GameBetMajor_Command()
	{
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new Common.ViewModels.GameBetMajorCommandViewModel
		{
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
			CurrencyRatio = 1,
			Bet = 100,
			ValidBet = 50,
			Settle = 97.5m,
			WinLoss = 47.5m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}
		};

		var response = new GameBetMajorInfoResponse(
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
			ExternalRoundId: Guid.NewGuid().ToString(),
			ExternalBetId: externalCreatedOn.ToUnixTimeMilliseconds().ToString(),
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
			CurrencyRatio: source.CurrencyRatio,
			Bet: source.Bet,
			ValidBet: source.ValidBet,
			Settle: source.Settle,
			WinLoss: source.WinLoss,
			Commission: source.Commission,
			Contribute: source.Contribute,
			Jackpot: source.Jackpot,
			CreatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 51, TimeSpan.FromHours(+8)),
			ClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 56, TimeSpan.FromHours(+8)),
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 57, TimeSpan.FromHours(+8)),
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalCreatedOn.ToUnixTimeMilliseconds().ToString()
			}.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();
			_ = fakeMediator.Send(
				request: Arg.Is<GameBetMajorCommandRequest>(compare =>
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
				compare.Commission == source.Commission &&
				compare.Contribute == source.Contribute &&
				compare.Jackpot == source.Jackpot &&
				compare.AdditionalData != null),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetMajorCommandAsync(source);

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
		Assert.Equal(response.WinLoss, actual.WinLoss);
		Assert.Equal(response.Commission, actual.Commission);
		Assert.Equal(response.Contribute, actual.Contribute);
		Assert.Equal(response.Jackpot, actual.Jackpot);
		Assert.Equal(response.CreatedOn, actual.CreatedOn);
		Assert.Equal(response.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(response.ClosedOn, actual.ClosedOn);
		Assert.NotNull(response.AdditionalData);
		Assert.NotNull(response.ExtendedData);
	}

	[Fact]
	public async Task GameBetMajor_Get_Id_ExternalCreatedOn()
	{
		var id = Guid.NewGuid();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var response = new GameBetMajorInfoResponse(
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
			ExternalPayoffOn: new DateTimeOffset(2024, 04, 23, 11, 40, 55, TimeSpan.FromHours(+8)),
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
			WinLoss: 47.5m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 51, TimeSpan.FromHours(+8)),
			ClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 56, TimeSpan.FromHours(+8)),
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 57, TimeSpan.FromHours(+8)),
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalCreatedOn.ToUnixTimeMilliseconds().ToString()
			}.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetMajorGetIdRequest(
					ExternalCreatedOn: externalCreatedOn,
					Id: id)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetMajorGetAsync(externalCreatedOn, id);

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
		Assert.Equal(response.WinLoss, actual.WinLoss);
		Assert.Equal(response.Commission, actual.Commission);
		Assert.Equal(response.Contribute, actual.Contribute);
		Assert.Equal(response.Jackpot, actual.Jackpot);
		Assert.Equal(response.CreatedOn, actual.CreatedOn);
		Assert.Equal(response.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(response.ClosedOn, actual.ClosedOn);
		Assert.NotNull(response.AdditionalData);
		Assert.NotNull(response.ExtendedData);
	}

	[Fact]
	public async Task GameBetMajor_Get_Id_ExternalCreatedOn_Data_Is_Empty()
	{
		var id = Guid.NewGuid();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetMajorGetIdRequest(
					ExternalCreatedOn: externalCreatedOn,
					Id: id)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetMajorInfoResponse?>(null));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetGetAsync(externalCreatedOn, id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task GameBetMajor_Get_External()
	{
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 23, 11, 40, 40, TimeSpan.FromHours(+8));

		var response = new GameBetMajorInfoResponse(
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
			ExternalRoundId: Guid.NewGuid().ToString(),
			ExternalBetId: externalCreatedOn.ToUnixTimeMilliseconds().ToString(),
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 50, TimeSpan.FromHours(+8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 04, 23, 11, 40, 55, TimeSpan.FromHours(+8)),
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
			WinLoss: 47.5m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 51, TimeSpan.FromHours(+8)),
			ClosedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 56, TimeSpan.FromHours(+8)),
			UpdatedOn: new DateTimeOffset(2024, 04, 23, 11, 40, 57, TimeSpan.FromHours(+8)),
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalCreatedOn.ToUnixTimeMilliseconds().ToString()
			}.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new GameBetMajorGetExternalRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetMajorInfoResponse?>(response));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetMajorGetAsync(
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
		Assert.Equal(response.WinLoss, actual.WinLoss);
		Assert.Equal(response.Commission, actual.Commission);
		Assert.Equal(response.Contribute, actual.Contribute);
		Assert.Equal(response.Jackpot, actual.Jackpot);
		Assert.Equal(response.CreatedOn, actual.CreatedOn);
		Assert.Equal(response.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(response.ClosedOn, actual.ClosedOn);
		Assert.NotNull(response.AdditionalData);
		Assert.NotNull(response.ExtendedData);
	}

	[Fact]
	public async Task GameBetMajor_Get_External_Data_Is_Empty()
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
				request: Arg.Is(new GameBetMajorGetExternalRequest(
					ExternalCreatedOn: externalCreatedOn,
					GameProviderId: gameProviderId,
					GameId: gameId,
					WalletId: walletId,
					ExternalBetId: externalBetId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<GameBetMajorInfoResponse?>(null));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		var actual = await client.GameBetMajorGetAsync(
			externalCreatedOn: externalCreatedOn,
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId);

		Assert.Null(actual);
	}
}
