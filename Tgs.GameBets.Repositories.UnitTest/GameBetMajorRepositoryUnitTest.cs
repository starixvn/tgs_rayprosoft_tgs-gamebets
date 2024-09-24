using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using Tgs.GameBets.Core.Models.GameBetMajors;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories.UnitTest;

public class GameBetMajorRepositoryUnitTest
{
	[Fact]
	public async Task Add()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderConfigurationId = Guid.NewGuid();
		var resellerId = Guid.NewGuid();
		var licenseeId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var playerCode = "test";
		var currencyCode = "CNY";
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorAddModel(
			Id: id,
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: gameProviderConfigurationId,
			GameId: gameId,
			ResellerId: resellerId,
			LicenseeId: licenseeId,
			BrandId: brandId,
			PlayerCode: playerCode,
			CurrencyCode: currencyCode,
			WalletId: walletId,
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
			Bet: 1.0m,
			ValidBet: 1.0m,
			Settle: 2.0m,
			WinLoss: 1.0m,
			Turnover: 1.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		await sut.AddAsync(source);

		var actual = await fakeContext.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.Id == id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(source.GameProviderId, actual.GameProviderId);
		Assert.Equal(source.GameId, actual.GameId);
		Assert.Equal(source.WalletId, actual.WalletId);
		Assert.Equal(source.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(source.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(source.Id, actual.Id);
		Assert.Equal(source.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(source.ResellerId, actual.ResellerId);
		Assert.Equal(source.LicenseeId, actual.LicenseeId);
		Assert.Equal(source.BrandId, actual.BrandId);
		Assert.Equal(source.PlayerCode, actual.PlayerCode);
		Assert.Equal(source.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(source.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(source.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(source.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal((int)source.BetState, actual.BetState);
		Assert.Equal((int)source.BetType, actual.BetType);
		Assert.Equal((int)source.FreeGameState, actual.FreeGameState);
		Assert.Equal((int)source.GambleState, actual.GambleState);
		Assert.Equal((int)source.CashoutState, actual.CashoutState);
		Assert.Equal((int)source.ResettleState, actual.ResettleState);
		Assert.Equal((int)source.JackpotState, actual.JackpotState);
		Assert.Equal(source.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(source.Bet, actual.Bet);
		Assert.Equal(source.ValidBet, actual.ValidBet);
		Assert.Equal(source.Settle, actual.Settle);
		Assert.Equal(source.WinLoss, actual.WinLoss);
		Assert.Equal(source.Turnover, actual.Turnover);
		Assert.Equal(source.Commission, actual.Commission);
		Assert.Equal(source.Contribute, actual.Contribute);
		Assert.Equal(source.Jackpot, actual.Jackpot);
		Assert.Equal(currentTime, actual.CreatedOn);
		Assert.Equal(currentTime, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
		Assert.Equal(source.ExtendedData.ToJsonString(), actual.ExtendedData);
	}

	[Fact]
	public async Task Add_Is_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderConfigurationId = Guid.NewGuid();
		var resellerId = Guid.NewGuid();
		var licenseeId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var playerCode = "test";
		var currencyCode = "CNY";
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = gameProviderId,
			GameId = gameId,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = gameProviderConfigurationId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = "test",
			CurrencyCode = "CNY",
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorAddModel(
			Id: id,
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: gameProviderConfigurationId,
			GameId: gameId,
			ResellerId: resellerId,
			LicenseeId: licenseeId,
			BrandId: brandId,
			PlayerCode: playerCode,
			CurrencyCode: currencyCode,
			WalletId: walletId,
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
			Bet: 1.0m,
			ValidBet: 1.0m,
			Settle: 2.0m,
			WinLoss: 1.0m,
			Turnover: 1.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddAsync(source).AsTask());
	}

	[Fact]
	public async Task Closed()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = Guid.NewGuid(),
			GameId = 1,
			WalletId = Guid.NewGuid(),
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = Guid.NewGuid(),
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Running,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 0.0m,
			WinLoss = 0.0m,
			Turnover = 0.0m,
			Commission = 0.0m,
			Contribute = 0.0m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorClosedModel(
			Id: id,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UtcNow,
			ExternalPayoffOn: DateTimeOffset.UtcNow,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Enums.BetState.Win,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			Settle: 2.0m,
			WinLoss: 1.0m,
			Turnover: 1.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		await sut.ClosedAsync(source);

		var actual = await fakeContext.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.Id == id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajor.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajor.GameId, actual.GameId);
		Assert.Equal(gameBetMajor.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajor.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajor.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajor.Id, actual.Id);
		Assert.Equal(gameBetMajor.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajor.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajor.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajor.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajor.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajor.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajor.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(source.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(source.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(source.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal((int)source.BetState, actual.BetState);
		Assert.Equal((int)source.BetType, actual.BetType);
		Assert.Equal((int)source.FreeGameState, actual.FreeGameState);
		Assert.Equal((int)source.GambleState, actual.GambleState);
		Assert.Equal((int)source.CashoutState, actual.CashoutState);
		Assert.Equal((int)source.ResettleState, actual.ResettleState);
		Assert.Equal((int)source.JackpotState, actual.JackpotState);
		Assert.Equal(source.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(source.Bet, actual.Bet);
		Assert.Equal(source.ValidBet, actual.ValidBet);
		Assert.Equal(source.Settle, actual.Settle);
		Assert.Equal(source.WinLoss, actual.WinLoss);
		Assert.Equal(source.Turnover, actual.Turnover);
		Assert.Equal(source.Commission, actual.Commission);
		Assert.Equal(source.Contribute, actual.Contribute);
		Assert.Equal(source.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajor.CreatedOn, actual.CreatedOn);
		Assert.Equal(currentTime, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
		Assert.Equal(source.ExtendedData.ToJsonString(), actual.ExtendedData);
	}

	[Fact]
	public async Task Closed_Is_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = Guid.NewGuid(),
			GameId = 1,
			WalletId = Guid.NewGuid(),
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = Guid.NewGuid(),
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = DateTimeOffset.UtcNow.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorClosedModel(
			Id: id,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UtcNow,
			ExternalPayoffOn: DateTimeOffset.UtcNow,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Enums.BetState.Win,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			Settle: 2.0m,
			WinLoss: 1.0m,
			Turnover: 1.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ClosedAsync(source).AsTask());
	}

	[Fact]
	public async Task Get_External()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderConfigurationId = Guid.NewGuid();
		var resellerId = Guid.NewGuid();
		var licenseeId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var playerCode = "test";
		var currencyCode = "CNY";
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = gameProviderId,
			GameId = gameId,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = gameProviderConfigurationId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalBetId);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajor.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajor.GameId, actual.GameId);
		Assert.Equal(gameBetMajor.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajor.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajor.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajor.Id, actual.Id);
		Assert.Equal(gameBetMajor.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajor.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajor.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajor.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajor.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajor.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajor.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajor.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajor.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajor.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajor.BetState, (int)actual.BetState);
		Assert.Equal(gameBetMajor.BetType, (int)actual.BetType);
		Assert.Equal(gameBetMajor.FreeGameState, (int)actual.FreeGameState);
		Assert.Equal(gameBetMajor.GambleState, (int)actual.GambleState);
		Assert.Equal(gameBetMajor.CashoutState, (int)actual.CashoutState);
		Assert.Equal(gameBetMajor.ResettleState, (int)actual.ResettleState);
		Assert.Equal(gameBetMajor.JackpotState, (int)actual.JackpotState);
		Assert.Equal(gameBetMajor.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajor.Bet, actual.Bet);
		Assert.Equal(gameBetMajor.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajor.Settle, actual.Settle);
		Assert.Equal(gameBetMajor.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajor.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajor.Commission, actual.Commission);
		Assert.Equal(gameBetMajor.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajor.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajor.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetMajor.ClosedOn, actual.ClosedOn);
		Assert.Equal(gameBetMajor.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBetMajor.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajor.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Get_External_Is_Null()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderConfigurationId = Guid.NewGuid();
		var resellerId = Guid.NewGuid();
		var licenseeId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var playerCode = "test";
		var currencyCode = "CNY";
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = gameProviderId,
			GameId = gameId,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = gameProviderConfigurationId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, gameProviderId, gameId, walletId, Guid.NewGuid().ToString("N"));

		Assert.Null(actual);
	}

	[Fact]
	public async Task Get_Id()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderConfigurationId = Guid.NewGuid();
		var resellerId = Guid.NewGuid();
		var licenseeId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var playerCode = "test";
		var currencyCode = "CNY";
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = gameProviderId,
			GameId = gameId,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = gameProviderConfigurationId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, id);

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajor.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajor.GameId, actual.GameId);
		Assert.Equal(gameBetMajor.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajor.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajor.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajor.Id, actual.Id);
		Assert.Equal(gameBetMajor.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajor.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajor.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajor.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajor.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajor.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajor.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetMajor.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetMajor.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetMajor.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajor.BetState, (int)actual.BetState);
		Assert.Equal(gameBetMajor.BetType, (int)actual.BetType);
		Assert.Equal(gameBetMajor.FreeGameState, (int)actual.FreeGameState);
		Assert.Equal(gameBetMajor.GambleState, (int)actual.GambleState);
		Assert.Equal(gameBetMajor.CashoutState, (int)actual.CashoutState);
		Assert.Equal(gameBetMajor.ResettleState, (int)actual.ResettleState);
		Assert.Equal(gameBetMajor.JackpotState, (int)actual.JackpotState);
		Assert.Equal(gameBetMajor.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetMajor.Bet, actual.Bet);
		Assert.Equal(gameBetMajor.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajor.Settle, actual.Settle);
		Assert.Equal(gameBetMajor.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajor.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajor.Commission, actual.Commission);
		Assert.Equal(gameBetMajor.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajor.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajor.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetMajor.ClosedOn, actual.ClosedOn);
		Assert.Equal(gameBetMajor.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBetMajor.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetMajor.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Get_Id_Is_Null()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var gameProviderConfigurationId = Guid.NewGuid();
		var resellerId = Guid.NewGuid();
		var licenseeId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var playerCode = "test";
		var currencyCode = "CNY";
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = gameProviderId,
			GameId = gameId,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = gameProviderConfigurationId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, Guid.NewGuid());

		Assert.Null(actual);
	}

	[Fact]
	public async Task Reopen()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = Guid.NewGuid(),
			GameId = 1,
			WalletId = Guid.NewGuid(),
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = Guid.NewGuid(),
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = DateTimeOffset.UtcNow.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorReopenModel(
			Id: id,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UtcNow,
			ExternalPayoffOn: DateTimeOffset.UtcNow,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Enums.BetState.Loss,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			Settle: 0.0m,
			WinLoss: -1.0m,
			Turnover: 1.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		await sut.ReopenAsync(source);

		var actual = await fakeContext.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.Id == id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajor.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajor.GameId, actual.GameId);
		Assert.Equal(gameBetMajor.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajor.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajor.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajor.Id, actual.Id);
		Assert.Equal(gameBetMajor.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajor.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajor.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajor.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajor.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajor.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajor.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(source.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(source.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(source.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal((int)source.BetState, actual.BetState);
		Assert.Equal((int)source.BetType, actual.BetType);
		Assert.Equal((int)source.FreeGameState, actual.FreeGameState);
		Assert.Equal((int)source.GambleState, actual.GambleState);
		Assert.Equal((int)source.CashoutState, actual.CashoutState);
		Assert.Equal((int)source.ResettleState, actual.ResettleState);
		Assert.Equal((int)source.JackpotState, actual.JackpotState);
		Assert.Equal(source.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(source.Bet, actual.Bet);
		Assert.Equal(source.ValidBet, actual.ValidBet);
		Assert.Equal(source.Settle, actual.Settle);
		Assert.Equal(source.WinLoss, actual.WinLoss);
		Assert.Equal(source.Turnover, actual.Turnover);
		Assert.Equal(source.Commission, actual.Commission);
		Assert.Equal(source.Contribute, actual.Contribute);
		Assert.Equal(source.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajor.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetMajor.ClosedOn, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
		Assert.Equal(source.ExtendedData.ToJsonString(), actual.ExtendedData);
	}

	[Fact]
	public async Task Reopen_Is_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = Guid.NewGuid(),
			GameId = 1,
			WalletId = Guid.NewGuid(),
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = Guid.NewGuid(),
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Running,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 0.0m,
			WinLoss = 0.0m,
			Turnover = 0.0m,
			Commission = 0.0m,
			Contribute = 0.0m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorReopenModel(
			Id: id,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UtcNow,
			ExternalPayoffOn: DateTimeOffset.UtcNow,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Enums.BetState.Loss,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			Settle: 0.0m,
			WinLoss: -1.0m,
			Turnover: 1.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ReopenAsync(source).AsTask());
	}

	[Fact]
	public async Task Running()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = Guid.NewGuid(),
			GameId = 1,
			WalletId = Guid.NewGuid(),
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = Guid.NewGuid(),
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Running,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 0.0m,
			WinLoss = 0.0m,
			Turnover = 0.0m,
			Commission = 0.0m,
			Contribute = 0.0m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorRunningModel(
			Id: id,
			ExternalCreatedOn: externalCreatedOn,
			Bet: 2.0m,
			ValidBet: 2.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		await sut.RunningAsync(source);

		var actual = await fakeContext.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.Id == id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(gameBetMajor.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetMajor.GameId, actual.GameId);
		Assert.Equal(gameBetMajor.WalletId, actual.WalletId);
		Assert.Equal(gameBetMajor.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetMajor.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetMajor.Id, actual.Id);
		Assert.Equal(gameBetMajor.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetMajor.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetMajor.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetMajor.BrandId, actual.BrandId);
		Assert.Equal(gameBetMajor.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetMajor.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetMajor.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(DateTimeOffset.UnixEpoch.UtcDateTime, actual.ExternalClosedOn);
		Assert.Equal(DateTimeOffset.UnixEpoch.UtcDateTime, actual.ExternalPayoffOn);
		Assert.Equal(DateTimeOffset.UnixEpoch.UtcDateTime, actual.ExternalResettleOn);
		Assert.Equal(gameBetMajor.BetState, actual.BetState);
		Assert.Equal(gameBetMajor.BetType, actual.BetType);
		Assert.Equal(gameBetMajor.FreeGameState, actual.FreeGameState);
		Assert.Equal(gameBetMajor.GambleState, actual.GambleState);
		Assert.Equal(gameBetMajor.CashoutState, actual.CashoutState);
		Assert.Equal(gameBetMajor.ResettleState, actual.ResettleState);
		Assert.Equal(gameBetMajor.JackpotState, actual.JackpotState);
		Assert.Equal(gameBetMajor.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(source.Bet, actual.Bet);
		Assert.Equal(source.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetMajor.Settle, actual.Settle);
		Assert.Equal(gameBetMajor.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetMajor.Turnover, actual.Turnover);
		Assert.Equal(gameBetMajor.Commission, actual.Commission);
		Assert.Equal(gameBetMajor.Contribute, actual.Contribute);
		Assert.Equal(gameBetMajor.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetMajor.CreatedOn, actual.CreatedOn);
		Assert.Equal(DateTimeOffset.UnixEpoch.UtcDateTime, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
		Assert.Equal(source.ExtendedData.ToJsonString(), actual.ExtendedData);
	}

	[Fact]
	public async Task Running_Is_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetMajorRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;

		var gameBetMajor = new GameBetMajor
		{
			GameProviderId = Guid.NewGuid(),
			GameId = 1,
			WalletId = Guid.NewGuid(),
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			Id = id,
			GameProviderConfigurationId = Guid.NewGuid(),
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			ExternalRoundId = externalRoundId,
			ExternalClosedOn = externalClosedOn.UtcDateTime,
			ExternalPayoffOn = externalPayoffOn.UtcDateTime,
			ExternalResettleOn = externalResettleOn.UtcDateTime,
			BetState = (int)Common.Enums.BetState.Win,
			BetType = (int)Common.Enums.BetType.Normal,
			FreeGameState = (int)Common.Enums.FreeGameState.None,
			GambleState = (int)Common.Enums.GambleState.None,
			CashoutState = (int)Common.Enums.CashoutState.None,
			ResettleState = (int)Common.Enums.ResettleState.None,
			JackpotState = (int)Common.Enums.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			Settle = 2.0m,
			WinLoss = 1.0m,
			Turnover = 1.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = DateTimeOffset.UtcNow.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString(),
		};
		_ = await fakeContext.GameBetMajors.AddAsync(gameBetMajor);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetMajorRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetMajorRunningModel(
			Id: id,
			ExternalCreatedOn: externalCreatedOn,
			Bet: 2.0m,
			ValidBet: 2.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RunningAsync(source).AsTask());
	}
}
