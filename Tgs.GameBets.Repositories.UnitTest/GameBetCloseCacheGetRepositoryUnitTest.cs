using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetCloses;

namespace Tgs.GameBets.Repositories.UnitTest;

public class GameBetCloseCacheGetRepositoryUnitTest
{
	[Fact]
	public async Task GetInfoWithGPCode_CacheMiss()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var model = new GameBetCloseInfoModel(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			UpdatedOn: DateTimeOffset.UtcNow.AddSeconds(-6),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			model.GameProviderId,
			":",
			model.GameId,
			":",
			model.WalletId,
			":",
			model.ExternalBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(model);

		_ = fakeDatabase
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None))
			.Returns(RedisValue.Null);

		_ = fakeDatabase
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget))
			.Returns(true);

		_ = fakeRepository
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameProviderId: Arg.Is(model.GameProviderId),
				gameId: Arg.Is(model.GameId),
				walletId: Arg.Is(model.WalletId),
				externalBetId: Arg.Is(model.ExternalBetId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetCloseInfoModel?>(model));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		var actual = await sut.GetAsync(
			closedOn: model.ClosedOn,
			gameProviderId: model.GameProviderId,
			gameId: model.GameId,
			walletId: model.WalletId,
			externalBetId: model.ExternalBetId,
			cancellationToken: default);

		_ = fakeDatabase
			.Received(1)
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None));

		_ = fakeDatabase
			.Received(1)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameProviderId: Arg.Is(model.GameProviderId),
				gameId: Arg.Is(model.GameId),
				walletId: Arg.Is(model.WalletId),
				externalBetId: Arg.Is(model.ExternalBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		Assert.NotNull(actual);
		Assert.Equal(model.Id, actual.Id);
		Assert.Equal(model.GameBetId, actual.GameBetId);
		Assert.Equal(model.GameProviderId, actual.GameProviderId);
		Assert.Equal(model.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(model.GameId, actual.GameId);
		Assert.Equal(model.ResellerId, actual.ResellerId);
		Assert.Equal(model.LicenseeId, actual.LicenseeId);
		Assert.Equal(model.BrandId, actual.BrandId);
		Assert.Equal(model.PlayerCode, actual.PlayerCode);
		Assert.Equal(model.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(model.WalletId, actual.WalletId);
		Assert.Equal(model.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(model.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(model.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(model.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(model.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(model.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(model.BetState, actual.BetState);
		Assert.Equal(model.BetType, actual.BetType);
		Assert.Equal(model.FreeGameState, actual.FreeGameState);
		Assert.Equal(model.GambleState, actual.GambleState);
		Assert.Equal(model.CashoutState, actual.CashoutState);
		Assert.Equal(model.ResettleState, actual.ResettleState);
		Assert.Equal(model.JackpotState, actual.JackpotState);
		Assert.Equal(model.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(model.Bet, actual.Bet);
		Assert.Equal(model.ValidBet, actual.ValidBet);
		Assert.Equal(model.Settle, actual.Settle);
		Assert.Equal(model.WinLoss, actual.WinLoss);
		Assert.Equal(model.Turnover, actual.Turnover);
		Assert.Equal(model.Commission, actual.Commission);
		Assert.Equal(model.Contribute, actual.Contribute);
		Assert.Equal(model.Jackpot, actual.Jackpot);
		Assert.Equal(model.CreatedOn, actual.CreatedOn);
		Assert.Equal(model.ClosedOn, actual.ClosedOn);
		Assert.Equal(model.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(model.ResetState, actual.ResetState);
		Assert.Equal(model.ResetOn, actual.ResetOn);
		Assert.Equal(model.AdditionalData, actual.AdditionalData);
		Assert.Equal(model.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task GetInfoWithBetId_CacheMiss()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var model = new GameBetCloseInfoModel(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			UpdatedOn: DateTimeOffset.UtcNow.AddSeconds(-6),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			model.GameBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(model);

		_ = fakeDatabase
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None))
			.Returns(RedisValue.Null);

		_ = fakeDatabase
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget))
			.Returns(true);

		_ = fakeRepository
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameBetId: Arg.Is(model.GameBetId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetCloseInfoModel?>(model));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		var actual = await sut.GetAsync(
			closedOn: model.ClosedOn,
			gameBetId: model.GameBetId,
			cancellationToken: default);

		_ = fakeDatabase
			.Received(1)
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None));

		_ = fakeDatabase
			.Received(1)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameBetId: Arg.Is(model.GameBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		Assert.NotNull(actual);
		Assert.Equal(model.Id, actual.Id);
		Assert.Equal(model.GameBetId, actual.GameBetId);
		Assert.Equal(model.GameProviderId, actual.GameProviderId);
		Assert.Equal(model.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(model.GameId, actual.GameId);
		Assert.Equal(model.ResellerId, actual.ResellerId);
		Assert.Equal(model.LicenseeId, actual.LicenseeId);
		Assert.Equal(model.BrandId, actual.BrandId);
		Assert.Equal(model.PlayerCode, actual.PlayerCode);
		Assert.Equal(model.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(model.WalletId, actual.WalletId);
		Assert.Equal(model.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(model.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(model.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(model.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(model.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(model.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(model.BetState, actual.BetState);
		Assert.Equal(model.BetType, actual.BetType);
		Assert.Equal(model.FreeGameState, actual.FreeGameState);
		Assert.Equal(model.GambleState, actual.GambleState);
		Assert.Equal(model.CashoutState, actual.CashoutState);
		Assert.Equal(model.ResettleState, actual.ResettleState);
		Assert.Equal(model.JackpotState, actual.JackpotState);
		Assert.Equal(model.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(model.Bet, actual.Bet);
		Assert.Equal(model.ValidBet, actual.ValidBet);
		Assert.Equal(model.Settle, actual.Settle);
		Assert.Equal(model.WinLoss, actual.WinLoss);
		Assert.Equal(model.Turnover, actual.Turnover);
		Assert.Equal(model.Commission, actual.Commission);
		Assert.Equal(model.Contribute, actual.Contribute);
		Assert.Equal(model.Jackpot, actual.Jackpot);
		Assert.Equal(model.CreatedOn, actual.CreatedOn);
		Assert.Equal(model.ClosedOn, actual.ClosedOn);
		Assert.Equal(model.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(model.ResetState, actual.ResetState);
		Assert.Equal(model.ResetOn, actual.ResetOn);
		Assert.Equal(model.AdditionalData, actual.AdditionalData);
		Assert.Equal(model.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task GetInfoWithGPCode_CacheHit()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var model = new GameBetCloseInfoModel(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			UpdatedOn: DateTimeOffset.UtcNow.AddSeconds(-6),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			model.GameProviderId,
			":",
			model.GameId,
			":",
			model.WalletId,
			":",
			model.ExternalBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(model);

		_ = fakeDatabase
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None))
			.Returns(Task.FromResult(value));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		var actual = await sut.GetAsync(
			closedOn: model.ClosedOn,
			gameProviderId: model.GameProviderId,
			gameId: model.GameId,
			walletId: model.WalletId,
			externalBetId: model.ExternalBetId,
			cancellationToken: default);

		_ = fakeDatabase
			.Received(1)
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None));

		_ = fakeDatabase
			.Received(0)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameProviderId: Arg.Is(model.GameProviderId),
				gameId: Arg.Is(model.GameId),
				walletId: Arg.Is(model.WalletId),
				externalBetId: Arg.Is(model.ExternalBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
				source: Arg.Any<GameBetCloseUpdateModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		Assert.NotNull(actual);
		Assert.Equal(model.Id, actual.Id);
		Assert.Equal(model.GameBetId, actual.GameBetId);
		Assert.Equal(model.GameProviderId, actual.GameProviderId);
		Assert.Equal(model.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(model.GameId, actual.GameId);
		Assert.Equal(model.ResellerId, actual.ResellerId);
		Assert.Equal(model.LicenseeId, actual.LicenseeId);
		Assert.Equal(model.BrandId, actual.BrandId);
		Assert.Equal(model.PlayerCode, actual.PlayerCode);
		Assert.Equal(model.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(model.WalletId, actual.WalletId);
		Assert.Equal(model.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(model.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(model.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(model.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(model.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(model.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(model.BetState, actual.BetState);
		Assert.Equal(model.BetType, actual.BetType);
		Assert.Equal(model.FreeGameState, actual.FreeGameState);
		Assert.Equal(model.GambleState, actual.GambleState);
		Assert.Equal(model.CashoutState, actual.CashoutState);
		Assert.Equal(model.ResettleState, actual.ResettleState);
		Assert.Equal(model.JackpotState, actual.JackpotState);
		Assert.Equal(model.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(model.Bet, actual.Bet);
		Assert.Equal(model.ValidBet, actual.ValidBet);
		Assert.Equal(model.Settle, actual.Settle);
		Assert.Equal(model.WinLoss, actual.WinLoss);
		Assert.Equal(model.Turnover, actual.Turnover);
		Assert.Equal(model.Commission, actual.Commission);
		Assert.Equal(model.Contribute, actual.Contribute);
		Assert.Equal(model.Jackpot, actual.Jackpot);
		Assert.Equal(model.CreatedOn, actual.CreatedOn);
		Assert.Equal(model.ClosedOn, actual.ClosedOn);
		Assert.Equal(model.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(model.ResetState, actual.ResetState);
		Assert.Equal(model.ResetOn, actual.ResetOn);
		Assert.Equal(model.AdditionalData, actual.AdditionalData);
		Assert.Equal(model.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task GetInfoWithBetId_CacheHit()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var model = new GameBetCloseInfoModel(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			UpdatedOn: DateTimeOffset.UtcNow.AddSeconds(-6),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			model.GameBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(model);

		_ = fakeDatabase
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None))
			.Returns(Task.FromResult(value));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		var actual = await sut.GetAsync(
			closedOn: model.ClosedOn,
			gameBetId: model.GameBetId,
			cancellationToken: default);

		_ = fakeDatabase
			.Received(1)
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None));

		_ = fakeDatabase
			.Received(0)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameBetId: Arg.Is(model.GameBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
				source: Arg.Any<GameBetCloseUpdateModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		Assert.NotNull(actual);
		Assert.Equal(model.Id, actual.Id);
		Assert.Equal(model.GameBetId, actual.GameBetId);
		Assert.Equal(model.GameProviderId, actual.GameProviderId);
		Assert.Equal(model.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(model.GameId, actual.GameId);
		Assert.Equal(model.ResellerId, actual.ResellerId);
		Assert.Equal(model.LicenseeId, actual.LicenseeId);
		Assert.Equal(model.BrandId, actual.BrandId);
		Assert.Equal(model.PlayerCode, actual.PlayerCode);
		Assert.Equal(model.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(model.WalletId, actual.WalletId);
		Assert.Equal(model.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(model.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(model.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(model.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(model.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(model.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(model.BetState, actual.BetState);
		Assert.Equal(model.BetType, actual.BetType);
		Assert.Equal(model.FreeGameState, actual.FreeGameState);
		Assert.Equal(model.GambleState, actual.GambleState);
		Assert.Equal(model.CashoutState, actual.CashoutState);
		Assert.Equal(model.ResettleState, actual.ResettleState);
		Assert.Equal(model.JackpotState, actual.JackpotState);
		Assert.Equal(model.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(model.Bet, actual.Bet);
		Assert.Equal(model.ValidBet, actual.ValidBet);
		Assert.Equal(model.Settle, actual.Settle);
		Assert.Equal(model.WinLoss, actual.WinLoss);
		Assert.Equal(model.Turnover, actual.Turnover);
		Assert.Equal(model.Commission, actual.Commission);
		Assert.Equal(model.Contribute, actual.Contribute);
		Assert.Equal(model.Jackpot, actual.Jackpot);
		Assert.Equal(model.CreatedOn, actual.CreatedOn);
		Assert.Equal(model.ClosedOn, actual.ClosedOn);
		Assert.Equal(model.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(model.ResetState, actual.ResetState);
		Assert.Equal(model.ResetOn, actual.ResetOn);
		Assert.Equal(model.AdditionalData, actual.AdditionalData);
		Assert.Equal(model.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task GetInfoWithGPCode_OccurException()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var model = new GameBetCloseInfoModel(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			UpdatedOn: DateTimeOffset.UtcNow.AddSeconds(-6),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			model.GameProviderId,
			":",
			model.GameId,
			":",
			model.WalletId,
			":",
			model.ExternalBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(model);

		_ = fakeDatabase
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None))
			.Returns(Task.FromException<RedisValue>(new Exception("test")));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await Assert.ThrowsAsync<Exception>(() => sut
			.GetAsync(
				closedOn: model.ClosedOn,
				gameProviderId: model.GameProviderId,
				gameId: model.GameId,
				walletId: model.WalletId,
				externalBetId: model.ExternalBetId,
				cancellationToken: default).AsTask())
			.ConfigureAwait(true);

		_ = fakeDatabase
			.Received(1)
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None));

		_ = fakeDatabase
			.Received(0)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameProviderId: Arg.Is(model.GameProviderId),
				gameId: Arg.Is(model.GameId),
				walletId: Arg.Is(model.WalletId),
				externalBetId: Arg.Is(model.ExternalBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
				source: Arg.Any<GameBetCloseUpdateModel>(),
				cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GetInfoWithBetId_OccurException()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var model = new GameBetCloseInfoModel(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			UpdatedOn: DateTimeOffset.UtcNow.AddSeconds(-6),
			ResetState: Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			model.GameBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(model);

		_ = fakeDatabase
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None))
			.Returns(Task.FromException<RedisValue>(new Exception("test")));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await Assert.ThrowsAsync<Exception>(() => sut
			.GetAsync(
				closedOn: model.ClosedOn,
				gameBetId: model.GameBetId,
				cancellationToken: default).AsTask())
			.ConfigureAwait(true);

		_ = fakeDatabase
			.Received(1)
			.StringGetAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.None));

		_ = fakeDatabase
			.Received(0)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
				closedOn: Arg.Is(model.ClosedOn),
				gameBetId: Arg.Is(model.GameBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
				source: Arg.Any<GameBetCloseUpdateModel>(),
				cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_ClearCache()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var source = new GameBetCloseUpdateModel(
			GameBetId: Guid.NewGuid(),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			source.GameBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(source);

		_ = fakeDatabase
			.KeyDeleteAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.FireAndForget))
			.Returns(Task.FromResult(true));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut
			.UpdateAsync(
				source: source,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeDatabase
			.Received(0)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
				closedOn: Arg.Is(source.ClosedOn),
				gameBetId: Arg.Is(source.GameBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
				source: Arg.Is(source),
				cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_ClearCache_OccurException()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheGetRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var source = new GameBetCloseUpdateModel(
			GameBetId: Guid.NewGuid(),
			ClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ExternalPayoffOn: DateTimeOffset.UtcNow.AddSeconds(-6),
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
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData: "{}");

		RedisKey key = string.Concat([
			typeof(GameBetCloseCacheGetRepository).Namespace!.Replace('.', '-').ToLower(),
			"bet-close",
			":",
			"cache",
			":",
			"get",
			source.GameBetId
		]);
		RedisValue value = JsonSerializer.SerializeToUtf8Bytes(source);

		_ = fakeDatabase
			.KeyDeleteAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.FireAndForget))
			.Returns(Task.FromException<bool>(new Exception("test")));

		var sut = new GameBetCloseCacheGetRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await Assert.ThrowsAsync<Exception>(() => sut
			.UpdateAsync(
				source: source,
				cancellationToken: default).AsTask())
			.ConfigureAwait(true);

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
				key: Arg.Is(key),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeDatabase
			.Received(0)
			.StringSetAsync(
				key: Arg.Is(key),
				value: Arg.Is(value),
				expiry: Arg.Is(fakeTimeSpan),
				when: Arg.Is(When.Always),
				flags: Arg.Is(CommandFlags.FireAndForget));

		_ = fakeRepository
			.Received(0)
			.AddAsync(
				source: Arg.Any<GameBetCloseAddModel>(),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
				closedOn: Arg.Is(source.ClosedOn),
				gameBetId: Arg.Is(source.GameBetId),
				cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
				source: Arg.Any<GameBetCloseUpdateModel>(),
				cancellationToken: Arg.Any<CancellationToken>());
	}
}
