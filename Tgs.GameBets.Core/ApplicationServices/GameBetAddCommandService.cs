using System.Text.Json.Nodes;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Core.ApplicationServices;

internal class GameBetAddCommandService(
	IGameBetRepository repository) : ICommandService<GameBetAddCommand>
{
	public async ValueTask ExecuteAsync(GameBetAddCommand command, CancellationToken cancellationToken = default)
	{
		await repository.AddAsync(
			source: new Models.GameBetAddModel(
				Id: command.GameBetId,
				GameProviderId: command.GameProviderId,
				GameProviderConfigurationId: command.GameProviderConfigurationId,
				GameId: command.GameId,
				ResellerId: command.ResellerId,
				LicenseeId: command.LicenseeId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				CurrencyCode: command.CurrencyCode,
				WalletId: command.WalletId,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: command.ExternalClosedOn,
				ExternalPayoffOn: command.ExternalPayoffOn,
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
				Jackpot: command.Jackpot),
			additionalData: JsonNode.Parse(command.AdditionalData)!,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
