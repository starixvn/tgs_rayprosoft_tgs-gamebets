using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBets;

namespace Tgs.GameBets.Repositories.UnitTest;

public class GameBetCacheLockedRepositoryUnitTest
{
	[Fact]
	public async Task Add_Cache_Is_Not_Locked()
	{
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetAddModel(
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
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50,
			Commission: 0.1m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		RedisKey key = string.Concat(typeof(GameBetCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", externalBetId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(true);

		var sut = new GameBetCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.AddAsync(source);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Any<GameBetAddModel>(),
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
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetAddModel(
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
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		RedisKey key = string.Concat(typeof(GameBetCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "cache", ":", "locked", source.GameProviderId, ":", source.GameProviderId, ":", source.WalletId, ":", externalBetId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(false);

		var sut = new GameBetCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.AddAsync(source);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeDatabase
			.Received(0)
			.KeyDeleteAsync(
			key: Arg.Any<RedisKey>(),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Update()
	{
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLog = NullLogger<GameBetCacheLockedRepository>.Instance;

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);

		var source = new GameBetUpdateModel(
			Id: Guid.NewGuid(),
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
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var sut = new GameBetCacheLockedRepository(fakeRepository, fakeDatabase, fakeTimeSpan, fakeLog);

		await sut.UpdateAsync(source);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
