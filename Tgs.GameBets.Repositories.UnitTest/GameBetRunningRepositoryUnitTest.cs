using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using Tgs.GameBets.Core.Models.GameBetRunnings;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories.UnitTest;

public class GameBetRunningRepositoryUnitTest
{
	[Fact]
	public async Task Add()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRunningRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var gameBetId = Guid.NewGuid();
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
		var createdOn = DateTime.UtcNow.AddSeconds(-5);

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetRunningRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetRunningAddModel(
			GameBetId: gameBetId,
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
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			CreatedOn: createdOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		await sut.AddAsync(source);

		var actual = await fakeContext.GameBetRunnings
			.Where(gbr => gbr.GameBetMajorId == gameBetId)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(source.GameBetId, actual.GameBetMajorId);
		Assert.Equal(source.GameProviderId, actual.GameProviderId);
		Assert.Equal(source.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(source.GameId, actual.GameId);
		Assert.Equal(source.ResellerId, actual.ResellerId);
		Assert.Equal(source.LicenseeId, actual.LicenseeId);
		Assert.Equal(source.BrandId, actual.BrandId);
		Assert.Equal(source.PlayerCode, actual.PlayerCode);
		Assert.Equal(source.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(source.WalletId, actual.WalletId);
		Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(source.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(source.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal((int)source.BetType, actual.BetType);
		Assert.Equal(source.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(source.Bet, actual.Bet);
		Assert.Equal(source.ValidBet, actual.ValidBet);
		Assert.Equal(source.CreatedOn, actual.CreatedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
	}

	[Fact]
	public async Task Add_Is_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRunningRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var gameBetId = Guid.NewGuid();
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
		var createdOn = DateTime.UtcNow.AddSeconds(-5);

		var gameBetRunning = new GameBetRunning
		{
			GameBetMajorId = gameBetId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			GameId = gameId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			ExternalRoundId = externalRoundId,
			BetType = (int)Common.Enums.BetType.Normal,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetRunnings.AddAsync(gameBetRunning);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetRunningRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetRunningAddModel(
			GameBetId: gameBetId,
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
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			CreatedOn: createdOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddAsync(source).AsTask());
	}

	[Fact]
	public async Task Delete()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRunningRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var gameBetId = Guid.NewGuid();
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

		var gameBetRunning = new GameBetRunning
		{
			GameBetMajorId = gameBetId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			GameId = gameId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			ExternalRoundId = externalRoundId,
			BetType = (int)Common.Enums.BetType.Normal,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetRunnings.AddAsync(gameBetRunning);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetRunningRepository(
			fakeContext,
			fakeTimeProvider);

		await sut.DeleteAsync(gameBetId);

		var actual = await fakeContext.GameBetRunnings
			.Where(gbr => gbr.GameBetMajorId == gameBetId)
			.FirstOrDefaultAsync();

		Assert.Null(actual);
	}

	[Fact]
	public async Task Get()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRunningRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var gameBetId = Guid.NewGuid();
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

		var gameBetRunning = new GameBetRunning
		{
			GameBetMajorId = gameBetId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			GameId = gameId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			ExternalRoundId = externalRoundId,
			BetType = (int)Common.Enums.BetType.Normal,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetRunnings.AddAsync(gameBetRunning);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetRunningRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(gameBetId);

		Assert.NotNull(actual);
		Assert.Equal(gameBetRunning.GameBetMajorId, actual.GameBetId);
		Assert.Equal(gameBetRunning.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetRunning.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetRunning.GameId, actual.GameId);
		Assert.Equal(gameBetRunning.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetRunning.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetRunning.BrandId, actual.BrandId);
		Assert.Equal(gameBetRunning.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetRunning.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetRunning.WalletId, actual.WalletId);
		Assert.Equal(gameBetRunning.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetRunning.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetRunning.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetRunning.BetType, (int)actual.BetType);
		Assert.Equal(gameBetRunning.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetRunning.Bet, actual.Bet);
		Assert.Equal(gameBetRunning.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetRunning.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetRunning.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBetRunning.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetRunning.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Get_Is_Null()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRunningRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var gameBetId = Guid.NewGuid();
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

		var gameBetRunning = new GameBetRunning
		{
			GameBetMajorId = gameBetId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			GameId = gameId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			ExternalRoundId = externalRoundId,
			BetType = (int)Common.Enums.BetType.Normal,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetRunnings.AddAsync(gameBetRunning);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetRunningRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(Guid.NewGuid());

		Assert.Null(actual);
	}

	[Fact]
	public async Task Update()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRunningRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var gameBetId = Guid.NewGuid();
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

		var gameBetRunning = new GameBetRunning
		{
			GameBetMajorId = gameBetId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			GameId = gameId,
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			CurrencyCode = currencyCode,
			WalletId = walletId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
			ExternalRoundId = externalRoundId,
			BetType = (int)Common.Enums.BetType.Normal,
			CurrencyRatio = 1.0m,
			Bet = 1.0m,
			ValidBet = 1.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			UpdatedOn = externalCreatedOn.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetRunnings.AddAsync(gameBetRunning);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetRunningRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetRunningUpdateModel(
			GameBetId: gameBetId,
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		await sut.UpdateAsync(source);

		var actual = await fakeContext.GameBetRunnings
			.Where(gbr => gbr.GameBetMajorId == gameBetId)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(gameBetRunning.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetRunning.GameId, actual.GameId);
		Assert.Equal(gameBetRunning.WalletId, actual.WalletId);
		Assert.Equal(gameBetRunning.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetRunning.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetRunning.GameBetMajorId, actual.GameBetMajorId);
		Assert.Equal(gameBetRunning.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetRunning.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetRunning.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetRunning.BrandId, actual.BrandId);
		Assert.Equal(gameBetRunning.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetRunning.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetRunning.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal((int)source.BetType, actual.BetType);
		Assert.Equal(source.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(source.Bet, actual.Bet);
		Assert.Equal(source.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetRunning.CreatedOn, actual.CreatedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
	}
}
