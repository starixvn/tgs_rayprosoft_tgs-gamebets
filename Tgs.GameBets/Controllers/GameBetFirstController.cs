using Microsoft.AspNetCore.Mvc;
using Tgs.GameBets.Core.ApplicationServices;

using Tgs.ServiceUtility;

namespace Tgs.GameBets.Controllers;

[Route("game-bet-first")]
[ApiController]
public class GameBetFirstController : ControllerBase
{
	/// <summary>
	/// Executes the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost(Name = "GameBetFirst_Command")]
	public ValueTask ExecuteAsync(
		[FromServices] ICommandService<GameBetFirstCommand> service,
		[FromBody] Common.ViewModels.GameBetFirstCommandViewModel source)
		=> service.ExecuteAsync(
			command: new GameBetFirstCommand(
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
				AdditionalData: source.AdditionalData),
			cancellationToken: HttpContext.RequestAborted);
}
