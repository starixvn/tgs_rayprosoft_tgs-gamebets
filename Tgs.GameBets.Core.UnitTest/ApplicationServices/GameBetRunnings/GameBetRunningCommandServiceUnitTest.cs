using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;
using Tgs.GameBets.Core.Models.GameBetRunnings;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetRunnings;

public class GameBetRunningCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_bet_adds_data()
	{
		var fakeLog = NullLogger<GameBetRunningCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningCommandRequest(
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
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			CreatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		_ = fakeRepository.GetAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetRunningInfoModel?>(null));

		var sut = new GameBetRunningCommandService(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is<GameBetRunningAddModel>(compare =>
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
				compare.BetType == request.BetType &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.CreatedOn == request.CreatedOn &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetRunningUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_update_bet_is_Change()
	{
		var fakeLog = NullLogger<GameBetRunningCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningCommandRequest(
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
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 50.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var gameBetRunningInfo = new GameBetRunningInfoModel(
			GameBetId: request.GameBetId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 50.0m,
			ValidBet: 50.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetRunningInfoModel?>(gameBetRunningInfo));

		var sut = new GameBetRunningCommandService(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetRunningAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Is<GameBetRunningUpdateModel>(compare =>
				compare.GameBetId == request.GameBetId &&
				compare.BetType == request.BetType &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_update_valid_bet_is_Change()
	{
		var fakeLog = NullLogger<GameBetRunningCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningCommandRequest(
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
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var gameBetRunningInfo = new GameBetRunningInfoModel(
			GameBetId: request.GameBetId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 50.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetRunningInfoModel?>(gameBetRunningInfo));

		var sut = new GameBetRunningCommandService(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetRunningAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Is<GameBetRunningUpdateModel>(compare =>
				compare.GameBetId == request.GameBetId &&
				compare.BetType == request.BetType &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_information_does_not_need_to_be_changed()
	{
		var fakeLog = NullLogger<GameBetRunningCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningCommandRequest(
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
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());

		var gameBetRunningInfo = new GameBetRunningInfoModel(
			GameBetId: request.GameBetId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: request.GameProviderConfigurationId,
			GameId: request.GameId,
			ResellerId: request.ResellerId,
			LicenseeId: request.LicenseeId,
			BrandId: request.BrandId,
			PlayerCode: request.PlayerCode,
			CurrencyCode: request.CurrencyCode,
			WalletId: request.WalletId,
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetType: Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToString(),
			ExtendedData: new JsonObject { }.ToJsonString());
		_ = fakeRepository.GetAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBetRunningInfoModel?>(gameBetRunningInfo));

		var sut = new GameBetRunningCommandService(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

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
	}
}
