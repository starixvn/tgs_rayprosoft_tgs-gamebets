using System.Text.Json.Nodes;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Core.ApplicationServices;

internal class GameBetUpdateCommandService(
	IGameBetRepository repository) : ICommandService<GameBetUpdateCommand>
{
	public async ValueTask ExecuteAsync(GameBetUpdateCommand command, CancellationToken cancellationToken = default)
	{
		await repository.UpdateAsync(
			source: new Models.GameBetUpdateModel(
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
				Jackpot: command.Jackpot),
			additionalData: JsonNode.Parse(command.AdditionalData)!,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
