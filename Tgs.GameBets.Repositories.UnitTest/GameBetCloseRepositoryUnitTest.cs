using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using Tgs.GameBets.Core.Models.GameBetCloses;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories.UnitTest;

public class GameBetCloseRepositoryUnitTest
{
	[Fact]
	public async Task Add()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetCloseRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
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
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetCloseRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetCloseAddModel(
			Id: id,
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
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		await sut.AddAsync(source);

		var actual = await fakeContext.GameBetCloses
			.Where(gbc => gbc.ClosedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbc.ClosedOn < DateTimeOffset.UtcNow &&
			gbc.GameBetMajorId == gameBetId)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(source.Id, actual.Id);
		Assert.Equal(source.GameBetId, actual.GameBetMajorId);
		Assert.Equal(source.GameProviderId, actual.GameProviderId);
		Assert.Equal(source.GameId, actual.GameId);
		Assert.Equal(source.WalletId, actual.WalletId);
		Assert.Equal(source.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(source.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(source.ResellerId, actual.ResellerId);
		Assert.Equal(source.LicenseeId, actual.LicenseeId);
		Assert.Equal(source.BrandId, actual.BrandId);
		Assert.Equal(source.PlayerCode, actual.PlayerCode);
		Assert.Equal(source.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(source.ExternalCreatedOn, actual.ExternalCreatedOn);
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
		Assert.Equal(createdOn, actual.CreatedOn);
		Assert.Equal(closedOn, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal((int)Common.Enums.ResetState.None, actual.ResetState);
		Assert.Equal(DateTimeOffset.UnixEpoch.UtcDateTime, actual.ResetOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
		Assert.Equal(source.ExtendedData.ToJsonString(), actual.ExtendedData);
	}

	[Fact]
	public async Task Add_Is_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetCloseRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
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
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var gameBetClose = new GameBetClose
		{
			Id = id,
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
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
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
			CreatedOn = createdOn.UtcDateTime,
			ClosedOn = closedOn.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetCloses.AddAsync(gameBetClose);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetCloseRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetCloseAddModel(
			Id: id,
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
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddAsync(source).AsTask());
	}

	[Fact]
	public async Task Get_External()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetCloseRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
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
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var gameBetClose = new GameBetClose
		{
			Id = id,
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
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
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
			CreatedOn = createdOn.UtcDateTime,
			ClosedOn = closedOn.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetCloses.AddAsync(gameBetClose);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetCloseRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalBetId);

		Assert.NotNull(actual);
		Assert.Equal(gameBetClose.Id, actual.Id);
		Assert.Equal(gameBetClose.GameBetMajorId, actual.GameBetId);
		Assert.Equal(gameBetClose.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetClose.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetClose.GameId, actual.GameId);
		Assert.Equal(gameBetClose.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetClose.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetClose.BrandId, actual.BrandId);
		Assert.Equal(gameBetClose.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetClose.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetClose.WalletId, actual.WalletId);
		Assert.Equal(gameBetClose.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetClose.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetClose.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetClose.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetClose.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetClose.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetClose.BetState, (int)actual.BetState);
		Assert.Equal(gameBetClose.BetType, (int)actual.BetType);
		Assert.Equal(gameBetClose.FreeGameState, (int)actual.FreeGameState);
		Assert.Equal(gameBetClose.GambleState, (int)actual.GambleState);
		Assert.Equal(gameBetClose.CashoutState, (int)actual.CashoutState);
		Assert.Equal(gameBetClose.ResettleState, (int)actual.ResettleState);
		Assert.Equal(gameBetClose.JackpotState, (int)actual.JackpotState);
		Assert.Equal(gameBetClose.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetClose.Bet, actual.Bet);
		Assert.Equal(gameBetClose.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetClose.Settle, actual.Settle);
		Assert.Equal(gameBetClose.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetClose.Turnover, actual.Turnover);
		Assert.Equal(gameBetClose.Commission, actual.Commission);
		Assert.Equal(gameBetClose.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetClose.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetClose.ClosedOn, actual.ClosedOn);
		Assert.Equal(gameBetClose.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBetClose.ResetState, (int)actual.ResetState);
		Assert.Equal(gameBetClose.ResetOn, actual.ResetOn);
		Assert.Equal(gameBetClose.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetClose.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Get_External_Is_Null()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetCloseRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
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
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var gameBetClose = new GameBetClose
		{
			Id = id,
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
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
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
			CreatedOn = createdOn.UtcDateTime,
			ClosedOn = closedOn.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetCloses.AddAsync(gameBetClose);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetCloseRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, gameProviderId, gameId, walletId, Guid.NewGuid().ToString("N"));

		Assert.Null(actual);
	}

	[Fact]
	public async Task Get_Id()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetCloseRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
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
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var gameBetClose = new GameBetClose
		{
			Id = id,
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
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
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
			CreatedOn = createdOn.UtcDateTime,
			ClosedOn = closedOn.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetCloses.AddAsync(gameBetClose);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetCloseRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, gameBetId);

		Assert.NotNull(actual);
		Assert.Equal(gameBetClose.Id, actual.Id);
		Assert.Equal(gameBetClose.GameBetMajorId, actual.GameBetId);
		Assert.Equal(gameBetClose.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetClose.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetClose.GameId, actual.GameId);
		Assert.Equal(gameBetClose.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetClose.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetClose.BrandId, actual.BrandId);
		Assert.Equal(gameBetClose.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetClose.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetClose.WalletId, actual.WalletId);
		Assert.Equal(gameBetClose.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetClose.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetClose.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(gameBetClose.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(gameBetClose.ExternalPayoffOn, actual.ExternalPayoffOn);
		Assert.Equal(gameBetClose.ExternalResettleOn, actual.ExternalResettleOn);
		Assert.Equal(gameBetClose.BetState, (int)actual.BetState);
		Assert.Equal(gameBetClose.BetType, (int)actual.BetType);
		Assert.Equal(gameBetClose.FreeGameState, (int)actual.FreeGameState);
		Assert.Equal(gameBetClose.GambleState, (int)actual.GambleState);
		Assert.Equal(gameBetClose.CashoutState, (int)actual.CashoutState);
		Assert.Equal(gameBetClose.ResettleState, (int)actual.ResettleState);
		Assert.Equal(gameBetClose.JackpotState, (int)actual.JackpotState);
		Assert.Equal(gameBetClose.CurrencyRatio, actual.CurrencyRatio);
		Assert.Equal(gameBetClose.Bet, actual.Bet);
		Assert.Equal(gameBetClose.ValidBet, actual.ValidBet);
		Assert.Equal(gameBetClose.Settle, actual.Settle);
		Assert.Equal(gameBetClose.WinLoss, actual.WinLoss);
		Assert.Equal(gameBetClose.Turnover, actual.Turnover);
		Assert.Equal(gameBetClose.Commission, actual.Commission);
		Assert.Equal(gameBetClose.Jackpot, actual.Jackpot);
		Assert.Equal(gameBetClose.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetClose.ClosedOn, actual.ClosedOn);
		Assert.Equal(gameBetClose.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameBetClose.ResetState, (int)actual.ResetState);
		Assert.Equal(gameBetClose.ResetOn, actual.ResetOn);
		Assert.Equal(gameBetClose.AdditionalData, actual.AdditionalData);
		Assert.Equal(gameBetClose.ExtendedData, actual.ExtendedData);
	}

	[Fact]
	public async Task Get_Id_Is_Null()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetCloseRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
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
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var gameBetClose = new GameBetClose
		{
			Id = id,
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
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
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
			CreatedOn = createdOn.UtcDateTime,
			ClosedOn = closedOn.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject { }.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetCloses.AddAsync(gameBetClose);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var sut = new GameBetCloseRepository(
			fakeContext,
			fakeTimeProvider);

		var actual = await sut.GetAsync(externalCreatedOn, Guid.NewGuid());

		Assert.Null(actual);
	}

	[Fact]
	public async Task Update()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		  .UseInMemoryDatabase(databaseName: $"{nameof(GameBetCloseRepositoryUnitTest)}.{Guid.NewGuid()}")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var fakeContext = new TgsContext(options);
		_ = fakeContext.Database.EnsureDeleted();

		var id = Guid.NewGuid();
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
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow.AddSeconds(-6);
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-3);

		var gameBetClose = new GameBetClose
		{
			Id = id,
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
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn.UtcDateTime,
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
			CreatedOn = createdOn.UtcDateTime,
			ClosedOn = closedOn.UtcDateTime,
			UpdatedOn = DateTimeOffset.UtcNow.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString(),
			ExtendedData = new JsonObject { }.ToJsonString()
		};
		_ = await fakeContext.GameBetCloses.AddAsync(gameBetClose);
		_ = await fakeContext.SaveChangesAsync();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var currentTime = DateTime.UtcNow.AddSeconds(5);
		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var sut = new GameBetCloseRepository(
			fakeContext,
			fakeTimeProvider);

		var source = new GameBetCloseUpdateModel(
			GameBetId: gameBetId,
			ClosedOn: externalCreatedOn,
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

		await sut.UpdateAsync(source);

		var actual = await fakeContext.GameBetCloses
			.Where(gbc => gbc.ClosedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbc.ClosedOn < DateTimeOffset.UtcNow &&
			gbc.GameBetMajorId == gameBetId)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(gameBetClose.Id, actual.Id);
		Assert.Equal(gameBetClose.GameBetMajorId, actual.GameBetMajorId);
		Assert.Equal(gameBetClose.GameProviderId, actual.GameProviderId);
		Assert.Equal(gameBetClose.GameId, actual.GameId);
		Assert.Equal(gameBetClose.WalletId, actual.WalletId);
		Assert.Equal(gameBetClose.ExternalBetId, actual.ExternalBetId);
		Assert.Equal(gameBetClose.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(gameBetClose.ResellerId, actual.ResellerId);
		Assert.Equal(gameBetClose.LicenseeId, actual.LicenseeId);
		Assert.Equal(gameBetClose.BrandId, actual.BrandId);
		Assert.Equal(gameBetClose.PlayerCode, actual.PlayerCode);
		Assert.Equal(gameBetClose.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(gameBetClose.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(gameBetClose.ExternalCreatedOn, actual.ExternalCreatedOn);
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
		Assert.Equal(gameBetClose.CreatedOn, actual.CreatedOn);
		Assert.Equal(gameBetClose.ClosedOn, actual.ClosedOn);
		Assert.Equal(currentTime, actual.UpdatedOn);
		Assert.Equal((int)Common.Enums.ResetState.Reset, actual.ResetState);
		Assert.Equal(currentTime, actual.ResetOn);
		Assert.Equal(source.AdditionalData.ToJsonString(), actual.AdditionalData);
		Assert.Equal(source.ExtendedData.ToJsonString(), actual.ExtendedData);
	}
}
