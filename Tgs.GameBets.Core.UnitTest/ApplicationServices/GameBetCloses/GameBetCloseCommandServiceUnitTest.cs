using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetCloses;
using Tgs.GameBets.Core.Models.GameBetCloses;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetCloses;

public class GameBetCloseCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_bet_adds_data()
	{
		var fakeLog = NullLogger<GameBetCloseCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();

		var gameBetId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddMinutes(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var request = new GameBetCloseCommandRequest(
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
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: externalResettleOn,
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
			Turnover: 50,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = fakeRepository.GetGameBetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(null));

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameBetCloseCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Equal(Common.Enums.GameBetCommandResult.Add, actual);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is<GameBetCloseAddModel>(compare =>
				compare.Id == id &&
				compare.GameBetId == request.GameBetId &&
				compare.GameProviderId == request.GameProviderId &&
				compare.GameProviderConfigurationId == request.GameProviderConfigurationId &&
				compare.GameId == request.GameId &&
				compare.ResellerId == request.ResellerId &&
				compare.LicenseeId == request.LicenseeId &&
				compare.BrandId == request.BrandId &&
				compare.PlayerCode == request.PlayerCode &&
				compare.CurrencyCode == request.CurrencyCode &&
				compare.WalletId == request.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == request.BetState &&
				compare.BetType == request.BetType &&
				compare.FreeGameState == request.FreeGameState &&
				compare.GambleState == request.GambleState &&
				compare.CashoutState == request.CashoutState &&
				compare.ResettleState == request.ResettleState &&
				compare.JackpotState == request.JackpotState &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.Settle == request.Settle &&
				compare.WinLoss == request.WinLoss &&
				compare.Turnover == request.Turnover &&
				compare.Commission == request.Commission &&
				compare.Contribute == request.Contribute &&
				compare.Jackpot == request.Jackpot &&
				compare.CreatedOn == request.CreatedOn &&
				compare.ClosedOn == request.ClosedOn &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_update_data()
	{
		var fakeLog = NullLogger<GameBetCloseCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();

		var gameBetId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddMinutes(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var request = new GameBetCloseCommandRequest(
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
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: externalResettleOn,
			BetState: Common.Enums.BetState.Win,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = fakeRepository.GetGameBetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(gameBetId));

		var gameBetCloseInfo = new GameBetCloseInfoModel(
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
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 0.0m,
			WinLoss: -50m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameBetId: Arg.Is(gameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetCloseInfo);

		var sut = new GameBetCloseCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Equal(Common.Enums.GameBetCommandResult.Update, actual);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Is<GameBetCloseUpdateModel>(compare =>
				compare.GameBetId == gameBetCloseInfo.GameBetId &&
				compare.ExternalCreatedOn == gameBetCloseInfo.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == request.BetState &&
				compare.BetType == request.BetType &&
				compare.FreeGameState == request.FreeGameState &&
				compare.GambleState == request.GambleState &&
				compare.CashoutState == request.CashoutState &&
				compare.ResettleState == request.ResettleState &&
				compare.JackpotState == request.JackpotState &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.Settle == request.Settle &&
				compare.WinLoss == request.WinLoss &&
				compare.Turnover == request.Turnover &&
				compare.Commission == request.Commission &&
				compare.Contribute == request.Contribute &&
				compare.Jackpot == request.Jackpot &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_information_does_not_need_to_be_changed()
	{
		var fakeLog = NullLogger<GameBetCloseCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();

		var gameBetId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddMinutes(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var request = new GameBetCloseCommandRequest(
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
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: externalResettleOn,
			BetState: Common.Enums.BetState.Win,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = fakeRepository.GetGameBetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(gameBetId));

		var gameBetCloseInfo = new GameBetCloseInfoModel(
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
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameBetId: Arg.Is(gameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetCloseInfo);

		var sut = new GameBetCloseCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Equal(Common.Enums.GameBetCommandResult.None, actual);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_is_running()
	{
		var fakeLog = NullLogger<GameBetCloseCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();

		var gameBetId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddMinutes(-4);
		var closedOn = DateTimeOffset.UnixEpoch;

		var request = new GameBetCloseCommandRequest(
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
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: externalResettleOn,
			BetState: Common.Enums.BetState.Running,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 100.0m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetCloseCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Equal(Common.Enums.GameBetCommandResult.Running, actual);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_is_waiting()
	{
		var fakeLog = NullLogger<GameBetCloseCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();

		var gameBetId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddMinutes(-4);
		var closedOn = DateTimeOffset.UnixEpoch;

		var request = new GameBetCloseCommandRequest(
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
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: externalResettleOn,
			BetState: Common.Enums.BetState.Waiting,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 100.0m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetCloseCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.Equal(Common.Enums.GameBetCommandResult.Running, actual);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
