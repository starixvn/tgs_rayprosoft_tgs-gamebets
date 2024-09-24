using System.Text.Json.Nodes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameBets.Core.ApplicationServices.GameBets;

namespace Tgs.GameBets.Controllers;

[Route("game-bet")]
[ApiController]
public class GameBetController : ControllerBase
{
	/// <summary>
	/// Games the bet command asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost()]
	public async ValueTask<Common.Enums.GameBetCommandResult> GameBetCommandAsync(
		[FromServices] IMediator mediator,
		[FromBody] Common.ViewModels.GameBetCommandViewModel source)
		=> await mediator.Send(
			request: new GameBetCommandRequest(
				GameBetId: source.Id,
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
				AdditionalData: source.AdditionalData.ToJsonString()),
			cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);

	/// <summary>
	/// Games the bet get external asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("external-bet")]
	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetExternalAsync(
		[FromServices] IMediator mediator,
		[FromQuery] Common.ViewModels.GameBetGetExternalViewModel source)
	{
		var response = await mediator.Send(
			request: new GameBetGetExternalRequest(
				GameProviderId: source.GameProviderId,
				GameId: source.GameId,
				WalletId: source.WalletId,
				ExternalBetId: source.ExternalBetId,
				ExternalCreatedOn: source.ExternalCreatedOn),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new Common.ViewModels.GameBetInfoViewModel(
				Id: response.Id,
				GameProviderId: response.GameProviderId,
				GameProviderConfigurationId: response.GameProviderConfigurationId,
				GameId: response.GameId,
				ResellerId: response.ResellerId,
				LicenseeId: response.LicenseeId,
				BrandId: response.BrandId,
				PlayerCode: response.PlayerCode,
				CurrencyCode: response.CurrencyCode,
				WalletId: response.WalletId,
				ExternalRoundId: response.ExternalRoundId,
				ExternalBetId: response.ExternalBetId,
				ExternalCreatedOn: response.ExternalCreatedOn,
				ExternalClosedOn: response.ExternalClosedOn,
				ExternalPayoffOn: response.ExternalPayoffOn,
				ExternalResettleOn: response.ExternalResettleOn,
				BetState: response.BetState,
				BetType: response.BetType,
				FreeGameState: response.FreeGameState,
				GambleState: response.GambleState,
				CashoutState: response.CashoutState,
				ResettleState: response.ResettleState,
				JackpotState: response.JackpotState,
				CurrencyRatio: response.CurrencyRatio,
				Bet: response.Bet,
				ValidBet: response.ValidBet,
				Settle: response.Settle,
				WinLoss: response.WinLoss,
				Turnover: response.Turnover,
				Commission: response.Commission,
				Contribute: 0.0m,
				Jackpot: response.Jackpot,
				CreatedOn: response.CreatedOn,
				ClosedOn: response.ClosedOn,
				UpdatedOn: response.UpdatedOn,
				ResetState: response.ResetState,
				ResetOn: response.ResetOn,
				AdditionalData: response.AdditionalData,
				ExtendedData: new JsonObject { });
	}

	/// <summary>
	/// Games the bet get identifier asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="timeProvider">The time provider.</param>
	/// <param name="id">The identifier.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("{id:guid}")]
	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetIdAsync(
		[FromServices] IMediator mediator,
		[FromServices] TimeProvider timeProvider,
		Guid id,
		[FromQuery] Common.ViewModels.GameBetGetIdViewModel source)
	{
		var externalCreatedOn = source.ExternalCreatedOn ?? timeProvider.GetUtcNow().AddDays(-45);

		var response = await mediator.Send(
			request: new GameBetGetIdRequest(
				Id: id,
				ExternalCreatedOn: externalCreatedOn),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new Common.ViewModels.GameBetInfoViewModel(
				Id: response.Id,
				GameProviderId: response.GameProviderId,
				GameProviderConfigurationId: response.GameProviderConfigurationId,
				GameId: response.GameId,
				ResellerId: response.ResellerId,
				LicenseeId: response.LicenseeId,
				BrandId: response.BrandId,
				PlayerCode: response.PlayerCode,
				CurrencyCode: response.CurrencyCode,
				WalletId: response.WalletId,
				ExternalRoundId: response.ExternalRoundId,
				ExternalBetId: response.ExternalBetId,
				ExternalCreatedOn: response.ExternalCreatedOn,
				ExternalClosedOn: response.ExternalClosedOn,
				ExternalPayoffOn: response.ExternalPayoffOn,
				ExternalResettleOn: response.ExternalResettleOn,
				BetState: response.BetState,
				BetType: response.BetType,
				FreeGameState: response.FreeGameState,
				GambleState: response.GambleState,
				CashoutState: response.CashoutState,
				ResettleState: response.ResettleState,
				JackpotState: response.JackpotState,
				CurrencyRatio: response.CurrencyRatio,
				Bet: response.Bet,
				ValidBet: response.ValidBet,
				Settle: response.Settle,
				WinLoss: response.WinLoss,
				Turnover: response.Turnover,
				Commission: response.Commission,
				Contribute: 0.0m,
				Jackpot: response.Jackpot,
				CreatedOn: response.CreatedOn,
				ClosedOn: response.ClosedOn,
				UpdatedOn: response.UpdatedOn,
				ResetState: response.ResetState,
				ResetOn: response.ResetOn,
				AdditionalData: response.AdditionalData,
				ExtendedData: new JsonObject { });
	}
}
