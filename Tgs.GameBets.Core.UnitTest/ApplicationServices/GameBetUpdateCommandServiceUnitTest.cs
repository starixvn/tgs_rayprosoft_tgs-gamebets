using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices;

public class GameBetUpdateCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameBetRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalResettleOn = DateTimeOffset.UtcNow.AddMinutes(-1);
		var command = new GameBetUpdateCommand(
			GameBetId: Guid.NewGuid(),
			GameProviderId: Guid.NewGuid(),
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
			ExternalResettleOn: externalResettleOn,
			BetState: Common.Enums.BetState.Win,
			BetType: Common.Enums.BetType.Normal,
			FreeGameState: Common.Enums.FreeGameState.None,
			GambleState: Common.Enums.GambleState.None,
			CashoutState: Common.Enums.CashoutState.None,
			ResettleState: Common.Enums.ResettleState.None,
			JackpotState: Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			}.ToJsonString());

		var sut = new GameBetUpdateCommandService(
			fakeRepository);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Is(new Models.GameBetUpdateModel(
				Id: command.GameBetId,
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				WalletId: command.WalletId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalResettleOn: command.ExternalResettleOn,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				Jackpot: command.Jackpot)),
			additionalData: Arg.Any<JsonNode>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
