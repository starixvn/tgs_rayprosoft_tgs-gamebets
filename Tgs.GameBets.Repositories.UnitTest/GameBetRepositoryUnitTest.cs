using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using System.Text.Json.Nodes;
using Tgs.GameBets.Core.Models.GameBets;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories.UnitTest;

public class GameBetRepositoryUnitTest
{
	[Fact]
	public async Task Add()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRepositoryUnitTest)}.{Guid.NewGuid()}")
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

		var sut = new GameBetRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetAddModel(
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
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		await sut.AddAsync(source);

		var actual = await fakeContext.GameBets
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
		Assert.Equal(source.Jackpot, actual.Jackpot);
		Assert.Equal(currentTime, actual.CreatedOn);
		Assert.Equal(currentTime, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
	}

	[Fact]
	public async Task Add_Is_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRepositoryUnitTest)}.{Guid.NewGuid()}")
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

		var gameBets = new GameBet
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
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBets.AddAsync(gameBets);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetAddModel(
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
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddAsync(source).AsTask());
	}

	[Fact]
	public async Task Get_External()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRepositoryUnitTest)}.{Guid.NewGuid()}")
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

		var gameBet = new GameBet
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
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBets.AddAsync(gameBet);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalBetId);

		Assert.NotNull(actual);
		Assert.Equal(gameBet.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBet.GameId, actual.GameId);
		Assert.Equal(gameBet.WalletId, actual.WalletId);
		Assert.Equal(gameBet.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBet.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBet.Id, actual.Id);
		Assert.Equal(gameBet.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBet.ResellerId, actual.ResellerId);
		Assert.Equal(gameBet.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBet.BrandId, actual.BrandId);
		Assert.Equal(gameBet.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBet.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBet.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBet.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBet.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBet.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBet.BetState, (int)actual.BetState);
		Assert.Equal(gameBet.BetType, (int)actual.BetType);
		Assert.Equal(gameBet.FreeGameState, (int)actual.FreeGameState);
		Assert.Equal(gameBet.GambleState, (int)actual.GambleState);
		Assert.Equal(gameBet.CashoutState, (int)actual.CashoutState);
		Assert.Equal(gameBet.ResettleState, (int)actual.ResettleState);
		Assert.Equal(gameBet.JackpotState, (int)actual.JackpotState);
		Assert.Equal(gameBet.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBet.Bet, actual.Bet);
		Assert.Equal(gameBet.ValidBet, actual.ValidBet);
		Assert.Equal(gameBet.Settle, actual.Settle);
		Assert.Equal(gameBet.WinLoss, actual.WinLoss);
		Assert.Equal(gameBet.Turnover, actual.Turnover);
		Assert.Equal(gameBet.Commission, actual.Commission);
		Assert.Equal(gameBet.Jackpot, actual.Jackpot);
		Assert.Equal(gameBet.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBet.ClosedOn, actual.ClosedOn);
		Assert.Equal(gameBet.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBet.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Get_External_Is_Null()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRepositoryUnitTest)}.{Guid.NewGuid()}")
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

		var gameBet = new GameBet
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
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBets.AddAsync(gameBet);
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
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRepositoryUnitTest)}.{Guid.NewGuid()}")
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

		var gameBet = new GameBet
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
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBets.AddAsync(gameBet);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, id);

		Assert.NotNull(actual);
		Assert.Equal(gameBet.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBet.GameId, actual.GameId);
		Assert.Equal(gameBet.WalletId, actual.WalletId);
		Assert.Equal(gameBet.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBet.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBet.Id, actual.Id);
		Assert.Equal(gameBet.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBet.ResellerId, actual.ResellerId);
		Assert.Equal(gameBet.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBet.BrandId, actual.BrandId);
		Assert.Equal(gameBet.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBet.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBet.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBet.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBet.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBet.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBet.BetState, (int)actual.BetState);
		Assert.Equal(gameBet.BetType, (int)actual.BetType);
		Assert.Equal(gameBet.FreeGameState, (int)actual.FreeGameState);
		Assert.Equal(gameBet.GambleState, (int)actual.GambleState);
		Assert.Equal(gameBet.CashoutState, (int)actual.CashoutState);
		Assert.Equal(gameBet.ResettleState, (int)actual.ResettleState);
		Assert.Equal(gameBet.JackpotState, (int)actual.JackpotState);
		Assert.Equal(gameBet.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBet.Bet, actual.Bet);
		Assert.Equal(gameBet.ValidBet, actual.ValidBet);
		Assert.Equal(gameBet.Settle, actual.Settle);
		Assert.Equal(gameBet.WinLoss, actual.WinLoss);
		Assert.Equal(gameBet.Turnover, actual.Turnover);
		Assert.Equal(gameBet.Commission, actual.Commission);
		Assert.Equal(gameBet.Jackpot, actual.Jackpot);
		Assert.Equal(gameBet.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBet.ClosedOn, actual.ClosedOn);
		Assert.Equal(gameBet.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBet.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Get_Id_Is_Null()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRepositoryUnitTest)}.{Guid.NewGuid()}")
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

		var gameBet = new GameBet
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
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = externalClosedOn.UtcDateTime,
			UpdatedOn = externalClosedOn.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBets.AddAsync(gameBet);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, Guid.NewGuid());

		Assert.Null(actual);
	}

	[Fact]
	public async Task Update()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetRepositoryUnitTest)}.{Guid.NewGuid()}")
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

		var gameBet = new GameBet
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
			Jackpot = 0.0m,
			CreatedOn = externalCreatedOn.UtcDateTime,
			ClosedOn = DateTimeOffset.UtcNow.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBets.AddAsync(gameBet);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetUpdateModel(
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
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		await sut.UpdateAsync(source);

		var actual = await fakeContext.GameBets
			.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.Id == id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(gameBet.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBet.GameId, actual.GameId);
		Assert.Equal(gameBet.WalletId, actual.WalletId);
		Assert.Equal(gameBet.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBet.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBet.Id, actual.Id);
		Assert.Equal(gameBet.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBet.ResellerId, actual.ResellerId);
		Assert.Equal(gameBet.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBet.BrandId, actual.BrandId);
		Assert.Equal(gameBet.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBet.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBet.ExternalRoundId, actual.ExternalRoundId);
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
		Assert.Equal(source.Jackpot, actual.Jackpot);
		Assert.Equal(gameBet.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBet.ClosedOn, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
	}
}
