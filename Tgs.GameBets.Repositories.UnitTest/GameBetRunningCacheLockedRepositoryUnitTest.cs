using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetCloses;
using Tgs.GameBets.Core.Models.GameBetRunnings;

namespace Tgs.GameBets.Repositories.UnitTest;
public class GameBetRunningCacheLockedRepositoryUnitTest
{
	[Fact]
	public async Task Add_Cache_Is_Not_Locked()
	{
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetRunningCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var source = new GameBetRunningAddModel(
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
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		RedisKey key = string.Concat(typeof(GameBetRunningCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-running", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", externalBetId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(true);

		var sut = new GameBetRunningCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.AddAsync(source);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Any<GameBetRunningAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetRunningUpdateModel>(),
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
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetRunningCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var source = new GameBetRunningAddModel(
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
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		RedisKey key = string.Concat(typeof(GameBetRunningCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-running", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameProviderId, ":", source.WalletId, ":", externalBetId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(false);

		var sut = new GameBetRunningCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.AddAsync(source);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetRunningAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetRunningUpdateModel>(),
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
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetRunningCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetRunningAddModel(
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
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		RedisKey key = string.Concat(typeof(GameBetRunningCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet-running", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", externalBetId);

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

		var sut = new GameBetRunningCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.AddAsync(source).AsTask());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetRunningUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
			key: Arg.Is<RedisKey>(key),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Get_GameBetId()
	{
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetRunningCacheLockedRepository>.Instance;

		var gameBetId = Guid.NewGuid();

		var sut = new GameBetRunningCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.GetAsync(gameBetId);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetRunningAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.GetAsync(
			 gameBetId: Arg.Is(gameBetId),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetRunningUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update()
	{
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetRunningCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();

		var source = new GameBetRunningUpdateModel(
			GameBetId: Guid.NewGuid(),
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.UpdateAsync(source);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetRunningAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			gameBetId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Any<GameBetRunningUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
