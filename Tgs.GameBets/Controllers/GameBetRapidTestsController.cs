using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;

namespace Tgs.GameBets.Controllers;

[Route("game-bet-rapid-test")]
[ApiController]
public class GameBetRapidTestsController : ControllerBase
{
	/// <summary>
	/// Games the bet rapid test command asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///			{
	///				"gameProviderCode": "KY",
	///				"subGameProviderCode": "KY",
	///				"gameId": 1000017,
	///				"resellerCode": "TGS",
	///				"licenseeCode": "TGS",
	///				"brandCode": "GPI",
	///				"playerCode": "4ucmqplxb029",
	///				"username": "4ucmqplxb029",
	///				"currencyCode": "CNY",
	///				"walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	///				"externalRoundId": "50-1691469296-370404138-1",
	///				"externalBetId": "50-1691469296-370404138-1",
	///				"externalGameId": "910",
	///				"betState": "Win",
	///				"betType": "Normal",
	///				"freeGameState": "None",
	///				"gambleState": "None",
	///				"cashoutState": "None",
	///				"resettleState": "None",
	///				"jackpotState": "None",
	///				"currencyRatio": 1,
	///				"bet": 4.0,
	///				"validBet": 4.0,
	///				"settle": 8.0,
	///				"winLoss": 4.0,
	///				"turnover": 4.0,
	///				"commission": 0.0,
	///				"contribute": 0.0,
	///				"jackpot": 0
	///			}
	/// </remarks>
	[HttpPost()]
	public async ValueTask<bool> GameBetRapidTestCommandAsync(
		[FromServices] IMediator mediator,
		[FromBody] Common.ViewModels.GameBetRapidTestCommandViewModel source)
	{
		return await mediator.Send(
			request: new GameBetRapidTestCommandRequest(
				GameProviderCode: source.GameProviderCode,
				SubGameProviderCode: source.SubGameProviderCode,
				GameId: source.GameId,
				ResellerCode: source.ResellerCode,
				LicenseeCode: source.LicenseeCode,
				BrandCode: source.BrandCode,
				PlayerCode: source.PlayerCode,
				Username: source.Username,
				CurrencyCode: source.CurrencyCode,
				WalletId: source.WalletId,
				ExternalRoundId: source.ExternalRoundId,
				ExternalBetId: source.ExternalBetId,
				ExternalGameId: source.ExternalGameId,
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
				Turnover: source.Turnover,
				WinLoss: source.WinLoss,
				Commission: source.Commission,
				Contribute: source.Contribute,
				Jackpot: source.Jackpot),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Games the bet rapid test expire asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <returns></returns>
	[HttpPut("{gameProviderCode}/expire")]
	public async ValueTask GameBetRapidTestExpireAsync(
		[FromServices] IMediator mediator,
		string gameProviderCode)
		=> await mediator.Send(
			request: new GameBetRapidTestExpireCommandRequest(
				GameProviderCode: gameProviderCode),
			cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);

	/// <summary>
	/// Games the bet rapid test initialization asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <returns></returns>
	[HttpPut("{gameProviderCode}/init")]
	public async ValueTask GameBetRapidTestInitializationAsync(
		[FromServices] IMediator mediator,
		string gameProviderCode)
		=> await mediator.Send(
			request: new GameBetRapidTestInitializationCommandRequest(
				GameProviderCode: gameProviderCode),
			cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);
}
