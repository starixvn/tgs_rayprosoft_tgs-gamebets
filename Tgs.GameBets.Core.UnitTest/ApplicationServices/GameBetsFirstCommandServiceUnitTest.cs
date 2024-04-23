using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices;

public class GameBetsFirstCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_wager_adds_data()
	{
		var fakeLogger = NullLogger<GameBetFirstCommandService>.Instance;
		var fakeGameBetFirstRepositoryRepository = Substitute.For<IGameBetFirstRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetFirstCommand(
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
			CreatedOn: externalCreatedOn,
			ClosedOn: externalClosedOn,
			UpdatedOn: externalClosedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = fakeGameBetFirstRepositoryRepository.ExistAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameBetFirstCommandService(
			fakeLogger,
			fakeGameBetFirstRepositoryRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetFirstRepositoryRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new Models.GameBetFirstAddModel(
				Id: id,
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
				CreatedOn: command.CreatedOn,
				ClosedOn: command.ClosedOn,
				UpdatedOn: command.UpdatedOn,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_wager_information_already_exists()
	{
		var fakeLogger = NullLogger<GameBetFirstCommandService>.Instance;
		var fakeGameBetFirstRepositoryRepository = Substitute.For<IGameBetFirstRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetFirstCommand(
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
			CreatedOn: externalCreatedOn,
			ClosedOn: externalClosedOn,
			UpdatedOn: externalClosedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = fakeGameBetFirstRepositoryRepository.ExistAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameBetFirstCommandService(
			fakeLogger,
			fakeGameBetFirstRepositoryRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetFirstRepositoryRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.GameBetFirstAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
