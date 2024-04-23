using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.IntegrationUnitTest;

public class GameBetsFirstUnitTest
{
	[Fact]
	public async Task GameBetsFirst_Command()
	{
		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new Common.ViewModels.GameBetFirstCommandViewModel(
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
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
			Jackpot: 0.0m,
			CreatedOn: externalPayoffOn,
			ClosedOn: externalClosedOn,
			UpdatedOn: externalPayoffOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var web = new GameBetsApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<ICommandService<GameBetFirstCommand>>();
			_ = fakeService.ExecuteAsync(
				command: Arg.Is(new GameBetFirstCommand(
					GameBetId: source.GameBetId,
					GameProviderId: source.GameProviderId,
					GameProviderConfigurationId: source.GameProviderConfigurationId,
					GameId: source.GameId,
					ResellerId: source.ResellerId,
					LicenseeId: source.LicenseeId,
					BrandId: source.BrandId,
					PlayerCode: source.PlayerCode,
					CurrencyCode: source.CurrencyCode,
					WalletId: source.WalletId,
					ExternalRoundId: source.ExternalRoundId,
					ExternalBetId: source.ExternalBetId,
					ExternalCreatedOn: source.ExternalCreatedOn,
					ExternalClosedOn: source.ExternalClosedOn,
					ExternalPayoffOn: source.ExternalPayoffOn,
					ExternalResettleOn: source.ExternalResettleOn,
					BetState: source.BetState,
					BetType: source.BetType,
					FreeGameState: source.FreeGameState,
					GambleState: source.GambleState,
					CashoutState: source.CashoutState,
					ResettleState: source.ResettleState,
					JackpotState: source.JackpotState,
					CurrencyRatio: source.CurrencyRatio,
					Bet: source.Bet,
					ValidBet: source.ValidBet,
					Settle: source.Settle,
					Commission: source.Commission,
					Turnover: source.Turnover,
					WinLoss: source.WinLoss,
					Jackpot: source.Jackpot,
					CreatedOn: source.CreatedOn,
					ClosedOn: source.ClosedOn,
					UpdatedOn: source.UpdatedOn,
					AdditionalData: source.AdditionalData)),
					cancellationToken:
			Arg.Any<CancellationToken>());

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsClient(httpClient);

		await client.GameBetFirstCommandAsync(source);
	}
}
