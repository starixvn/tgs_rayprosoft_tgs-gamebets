using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices;

public class GameBetCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_bet_adds_data()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameBetAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameBetUpdateCommand>>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetCommand(
			GameBetId: Guid.NewGuid(),
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
			}.ToJsonString());

		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((Models.GameBetInfoModel?)null);

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeAddService,
			fakeUpdateService);

		var actual = await sut.GetAsync(command);

		Assert.Equal(Common.Enums.GameBetCommandResult.Add, actual);

		_ = fakeAddService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetAddCommand(
				GameBetId: command.GameBetId,
					GameProviderId: command.GameProviderId,
					GameProviderConfigurationId: command.GameProviderConfigurationId,
					GameId: command.GameId,
					ResellerId: command.ResellerId,
					LicenseeId: command.LicenseeId,
					BrandId: command.BrandId,
					PlayerCode: command.PlayerCode,
					CurrencyCode: command.CurrencyCode,
					WalletId: command.WalletId,
					ExternalRoundId: command.ExternalRoundId,
					ExternalBetId: command.ExternalBetId,
					ExternalCreatedOn: command.ExternalCreatedOn,
					ExternalClosedOn: command.ExternalClosedOn,
					ExternalPayoffOn: command.ExternalPayoffOn,
					ExternalResettleOn: command.ExternalResettleOn,
					BetState: command.BetState,
					BetType: command.BetType,
					FreeGameState: command.FreeGameState,
					GambleState: command.GambleState,
					CashoutState: command.CashoutState,
					ResettleState: command.ResettleState,
					JackpotState: command.JackpotState,
					CurrencyRatio: command.CurrencyRatio,
					Bet: command.Bet,
					ValidBet: command.ValidBet,
					Settle: command.Settle,
					Commission: command.Commission,
					Turnover: command.Turnover,
					WinLoss: command.WinLoss,
					Jackpot: command.Jackpot,
					AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetUpdateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_update_data()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameBetAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameBetUpdateCommand>>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetCommand(
			GameBetId: Guid.NewGuid(),
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
			}.ToJsonString());

		var model = new Models.GameBetInfoModel(
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
			BetState: Common.Enums.BetState.Loss,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 0.0m,
			Commission: 0,
			Turnover: 50,
			WinLoss: -50m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString());
		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeAddService,
			fakeUpdateService);

		var actual = await sut.GetAsync(command);

		Assert.Equal(Common.Enums.GameBetCommandResult.Update, actual);

		_ = fakeAddService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetAddCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetUpdateCommand(
				GameBetId: model.Id,
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				ResellerId: command.ResellerId,
				LicenseeId: command.LicenseeId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				CurrencyCode: command.CurrencyCode,
				WalletId: command.WalletId,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalResettleOn: command.ExternalResettleOn,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				Jackpot: command.Jackpot,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_information_does_not_need_to_be_changed()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameBetAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameBetUpdateCommand>>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetCommand(
			GameBetId: Guid.NewGuid(),
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
			}.ToJsonString());

		var model = new Models.GameBetInfoModel(
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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString());
		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeAddService,
			fakeUpdateService);

		var actual = await sut.GetAsync(command);

		Assert.Equal(Common.Enums.GameBetCommandResult.None, actual);

		_ = fakeAddService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetAddCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetUpdateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_is_running()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameBetAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameBetUpdateCommand>>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var command = new GameBetCommand(
			GameBetId: Guid.NewGuid(),
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
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Enums.BetState.Running,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 100,
			Settle: 0.0m,
			Commission: 0,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString());

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeAddService,
			fakeUpdateService);

		var actual = await sut.GetAsync(command);

		Assert.Equal(Common.Enums.GameBetCommandResult.Running, actual);

		_ = fakeAddService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetAddCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetUpdateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_is_waiting()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameBetAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameBetUpdateCommand>>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var command = new GameBetCommand(
			GameBetId: Guid.NewGuid(),
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
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Enums.BetState.Waiting,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 100,
			Settle: 0.0m,
			Commission: 0,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString());

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeAddService,
			fakeUpdateService);

		var actual = await sut.GetAsync(command);

		Assert.Equal(Common.Enums.GameBetCommandResult.Running, actual);

		_ = fakeAddService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetAddCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetUpdateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
