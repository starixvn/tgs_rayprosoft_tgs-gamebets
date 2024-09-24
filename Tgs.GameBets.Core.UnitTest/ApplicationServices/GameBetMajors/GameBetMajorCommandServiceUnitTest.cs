using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetMajors;
using Tgs.GameBets.Core.Models.GameBetMajors;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetMajors;

public class GameBetMajorCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_bet_adds_data()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: request.BetState,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: request.Bet,
			ValidBet: request.ValidBet,
			Settle: request.Settle,
			WinLoss: request.WinLoss,
			Turnover: request.Turnover,
			Commission: request.Commission,
			Contribute: request.Contribute,
			Jackpot: request.Jackpot,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(null),
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is<GameBetMajorAddModel>(compare =>
				compare.Id == id &&
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
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ClosedAsync(
			source: Arg.Any<GameBetMajorClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ReopenAsync(
			source: Arg.Any<GameBetMajorReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.RunningAsync(
			source: Arg.Any<GameBetMajorRunningModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_closed_data()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			Bet: 100m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfoBefore = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: Common.Enums.BetState.Running,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: request.Bet,
			ValidBet: request.ValidBet,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: request.Turnover,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: request.BetState,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: request.Bet,
			ValidBet: request.ValidBet,
			Settle: request.Settle,
			WinLoss: request.WinLoss,
			Turnover: request.Turnover,
			Commission: request.Commission,
			Contribute: request.Contribute,
			Jackpot: request.Jackpot,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfoBefore),
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetMajorAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.ClosedAsync(
			source: Arg.Is<GameBetMajorClosedModel>(compare =>
				compare.Id == id &&
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
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ReopenAsync(
			source: Arg.Any<GameBetMajorReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.RunningAsync(
			source: Arg.Any<GameBetMajorRunningModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_information_does_not_need_to_be_changed()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: request.BetState,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: request.Bet,
			ValidBet: request.ValidBet,
			Settle: request.Settle,
			WinLoss: request.WinLoss,
			Turnover: request.Turnover,
			Commission: request.Commission,
			Contribute: request.Contribute,
			Jackpot: request.Jackpot,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetMajorAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ClosedAsync(
			source: Arg.Any<GameBetMajorClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ReopenAsync(
			source: Arg.Any<GameBetMajorReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.RunningAsync(
			source: Arg.Any<GameBetMajorRunningModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_is_settle_and_request_bet_state_is_running()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 50.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: Common.Enums.BetState.Win,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetMajorAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ClosedAsync(
			source: Arg.Any<GameBetMajorClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ReopenAsync(
			source: Arg.Any<GameBetMajorReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.RunningAsync(
			source: Arg.Any<GameBetMajorRunningModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_is_settle_and_request_bet_state_is_waiting()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			Bet: 100.0m,
			ValidBet: 100.0m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: Common.Enums.BetState.Win,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetMajorAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ClosedAsync(
			source: Arg.Any<GameBetMajorClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ReopenAsync(
			source: Arg.Any<GameBetMajorReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.RunningAsync(
			source: Arg.Any<GameBetMajorRunningModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_running_data_bet_is_changed()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			BetState: Common.Enums.BetState.Running,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100m,
			ValidBet: 50.0m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfoBefore = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: Common.Enums.BetState.Running,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: 50.0m,
			ValidBet: request.ValidBet,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: request.Turnover,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: request.BetState,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: request.Bet,
			ValidBet: request.ValidBet,
			Settle: request.Settle,
			WinLoss: request.WinLoss,
			Turnover: request.Turnover,
			Commission: request.Commission,
			Contribute: request.Contribute,
			Jackpot: request.Jackpot,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfoBefore),
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetMajorAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ClosedAsync(
			source: Arg.Any<GameBetMajorClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ReopenAsync(
			source: Arg.Any<GameBetMajorReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.RunningAsync(
			source: Arg.Is<GameBetMajorRunningModel>(compare =>
				compare.Id == id &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_running_data_valid_bet_is_changed()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			BetState: Common.Enums.BetState.Running,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100m,
			ValidBet: 100m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfoBefore = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: Common.Enums.BetState.Running,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: 100m,
			ValidBet: 50.0m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 50.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: request.BetState,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: request.Bet,
			ValidBet: request.ValidBet,
			Settle: request.Settle,
			WinLoss: request.WinLoss,
			Turnover: request.Turnover,
			Commission: request.Commission,
			Contribute: request.Contribute,
			Jackpot: request.Jackpot,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfoBefore),
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetMajorAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ClosedAsync(
			source: Arg.Any<GameBetMajorClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ReopenAsync(
			source: Arg.Any<GameBetMajorReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.RunningAsync(
			source: Arg.Is<GameBetMajorRunningModel>(compare =>
				compare.Id == id &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_reopen_data()
	{
		var fakeLog = NullLogger<GameBetMajorCommandService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IGameBetMajorRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetMajorCommandRequest(
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
			Bet: 100m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>()).Returns(id);

		var gameBetMajorInfoBefore = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: Common.Enums.BetState.Loss,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: 100.0m,
			ValidBet: 100.0m,
			Settle: 0.0m,
			WinLoss: -100.0m,
			Turnover: 100.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		var gameBetMajorInfo = new GameBetMajorInfoModel(
			Id: id,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: request.ExternalRoundId,
			ExternalBetId: request.ExternalBetId,
			ExternalCreatedOn: request.ExternalCreatedOn,
			ExternalClosedOn: request.ExternalClosedOn,
			ExternalPayoffOn: request.ExternalPayoffOn,
			ExternalResettleOn: request.ExternalResettleOn,
			BetState: request.BetState,
			BetType: request.BetType,
			FreeGameState: request.FreeGameState,
			GambleState: request.GambleState,
			CashoutState: request.CashoutState,
			ResettleState: request.ResettleState,
			JackpotState: request.JackpotState,
			CurrencyRatio: request.CurrencyRatio,
			Bet: request.Bet,
			ValidBet: request.ValidBet,
			Settle: request.Settle,
			WinLoss: request.WinLoss,
			Turnover: request.Turnover,
			Commission: request.Commission,
			Contribute: request.Contribute,
			Jackpot: request.Jackpot,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: request.AdditionalData.ToJsonString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalBetId: Arg.Is(request.ExternalBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfoBefore),
			ValueTask.FromResult<GameBetMajorInfoModel?>(gameBetMajorInfo));

		var sut = new GameBetMajorCommandService(
			fakeLog,
			fakeSerialNumberGenerator,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajorInfo.Id, actual.Id);
		Assert.Equal(gameBetMajorInfo.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajorInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajorInfo.GameId, actual.GameId);
		Assert.Equal(gameBetMajorInfo.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajorInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajorInfo.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajorInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajorInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajorInfo.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajorInfo.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajorInfo.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajorInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajorInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajorInfo.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajorInfo.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajorInfo.BetState, actual.BetState);
		Assert.Equal(gameBetMajorInfo.BetType, actual.BetType);
		Assert.Equal(gameBetMajorInfo.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajorInfo.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajorInfo.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajorInfo.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajorInfo.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajorInfo.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajorInfo.Bet, actual.Bet);
		Assert.Equal(gameBetMajorInfo.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajorInfo.Settle, actual.Settle);
		Assert.Equal(gameBetMajorInfo.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajorInfo.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajorInfo.Commission, actual.Commission);
		Assert.Equal(gameBetMajorInfo.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajorInfo.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajorInfo.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajorInfo.ExtendedData, actual.ExtendedData);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetMajorAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.ClosedAsync(
			source: Arg.Any<GameBetMajorClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.ReopenAsync(
			source: Arg.Is<GameBetMajorReopenModel>(compare =>
				compare.Id == id &&
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
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.RunningAsync(
			source: Arg.Any<GameBetMajorRunningModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
