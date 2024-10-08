using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetCloses;

namespace Tgs.GameBets.Repositories.UnitTest;

public class GameBetCloseCacheLockedRepositoryUnitTest
{
	[Fact]
	public async Task Add_Cache_Is_Not_Locked()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetCloseAddModel(
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
			CreatedOn: externalCreatedOn,
			ClosedOn: externalClosedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		RedisKey key = string.Concat(typeof(GameBetCloseCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-close", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", externalBetId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(true);

		var sut = new GameBetCloseCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.AddAsync(source);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeDatabase
			.Received(0)
			.KeyDeleteAsync(
			key: Arg.Any<RedisKey>(),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Add_Cache_Is_Locked()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetCloseAddModel(
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
			CreatedOn: externalCreatedOn,
			ClosedOn: externalClosedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		RedisKey key = string.Concat(typeof(GameBetRunningCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-close", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameProviderId, ":", source.WalletId, ":", externalBetId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(false);

		var sut = new GameBetCloseCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.AddAsync(source);

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

		_ = fakeDatabase
			.Received(0)
			.KeyDeleteAsync(
			key: Arg.Any<RedisKey>(),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Add_Exception()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetCloseAddModel(
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
			CreatedOn: externalCreatedOn,
			ClosedOn: externalClosedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		RedisKey key = string.Concat(typeof(GameBetRunningCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-close", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", externalBetId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(true);

		_ = fakeRepository
			.AddAsync(
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException(new Exception("test")));

		var sut = new GameBetCloseCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.AddAsync(source).AsTask());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
			key: Arg.Is<RedisKey>(key),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Get_ExternalBetId()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheLockedRepository>.Instance;

		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = Guid.NewGuid().ToString();

		var sut = new GameBetCloseCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.GetAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalBetId);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.GetAsync(
			 externalCreatedOn: Arg.Is(externalCreatedOn),
			 gameProviderId: Arg.Is(gameProviderId),
			 gameId: Arg.Is(gameId),
			 walletId: Arg.Is(walletId),
			 externalBetId: Arg.Is(externalBetId),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			 externalCreatedOn: Arg.Any<DateTimeOffset>(),
			 gameBetId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Get_GameBetId()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheLockedRepository>.Instance;

		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameBetId = Guid.NewGuid();

		var sut = new GameBetCloseCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.GetAsync(externalCreatedOn, gameBetId);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			 gameProviderId: Arg.Any<Guid>(),
			 gameId: Arg.Any<int>(),
			 walletId: Arg.Any<Guid>(),
			 externalBetId: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.GetAsync(
			 externalCreatedOn: Arg.Is(externalCreatedOn),
			 gameBetId: Arg.Is(gameBetId),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update()
	{
		var fakeRepository = Substitute.For<IGameBetCloseRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCloseCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-1);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetCloseUpdateModel(
			GameBetId: Guid.NewGuid(),
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

			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetCloseCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.UpdateAsync(source);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetCloseAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			gameProviderId: Arg.Any<Guid>(),
			gameId: Arg.Any<int>(),
			walletId: Arg.Any<Guid>(),
			externalBetId: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			gameBetId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Any<GameBetCloseUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
