using System.Text.Json.Nodes;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameBets.Core.ApplicationServices.GameBetCloses;

namespace Tgs.GameBets.Controllers;

[Route("game-bet-closes")]
[ApiController]
public class GameBetClosesController : ControllerBase
{
	/// <summary>
	/// Games the bet close command asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///		Post
	///		{
	///			"gameBetId": "0191e3f1-e185-0000-0001-07e8778bb552",
	///			"gameProviderId": "147cef56-4fcd-4ee3-b748-87ad398f2871",
	///			"gameProviderConfigurationId": "dec3dec2-ac0f-4f0d-a12e-eec0bc9e083a",
	///			"gameId": 6700018,
	///			"resellerId": "b5a2c708-ab70-4316-9f4d-5d2ad0e77460",
	///			"licenseeId": "66c5fef5-217b-4929-9a08-72c2479a8f4e",
	///			"brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
	///			"playerCode": "4unykr72x8d9",
	///			"currencyCode": "CNY",
	///			"walletId": "8bff49a2-66ff-4c66-81f6-c9b242f2b868",
	///			"externalRoundId": "6323442723",
	///			"externalBetId": "6323442723",
	///			"externalCreatedOn": "2024-09-12T09:55:12.737+08:00",
	///			"externalClosedOn": "2024-09-12T09:55:15.555+08:00",
	///			"externalPayoffOn": "2024-09-12T09:55:15.555+08:00",
	///			"externalResettleOn": "1970-01-01T08:00:00+08:00",
	///			"betState": "Win",
	///			"betType": "Normal",
	///			"freeGameState": "None",
	///			"gambleState": "None",
	///			"cashoutState": "None",
	///			"resettleState": "None",
	///			"jackpotState": "None",
	///			"currencyRatio": 1,
	///			"bet": 0.8,
	///			"validBet": 0.8,
	///			"settle": 1.2,
	///			"winLoss": 0.4,
	///			"turnover": 0.8,
	///			"commission": 0.01,
	///			"contribute": 0.001,
	///			"jackpot": 0,
	///			"createdOn": "2024-09-12T09:56:05+08:00",
	///			"closedOn": "2024-09-12T09:56:05+08:00",
	///			"extendedData": {}
	///		}
	/// </remarks>
	[HttpPost()]
	public async ValueTask<Common.Enums.GameBetCommandResult> GameBetCloseCommandAsync(
		[FromServices] IMediator mediator,
		[FromBody] Common.ViewModels.GameBetCloseCommandViewModel source)
		=> await mediator.Send(
			request: new GameBetCloseCommandRequest(
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
				WinLoss: source.WinLoss,
				Turnover: source.Turnover,
				Commission: source.Commission,
				Contribute: source.Contribute,
				Jackpot: source.Jackpot,
				CreatedOn: source.CreatedOn,
				ClosedOn: source.ClosedOn,
				AdditionalData: source.AdditionalData,
				ExtendedData: source.ExtendedData),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

	/// <summary>
	/// Games the bet close get external asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("external-bet")]
	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetCloseGetExternalAsync(
		[FromServices] IMediator mediator,
		[FromQuery] Common.ViewModels.GameBetCloseGetExternalViewModel source)
	{
		var response = await mediator.Send(
			request: new GameBetCloseGetExternalRequest(
				ExternalCreatedOn: source.ExternalCreatedOn,
				GameProviderId: source.GameProviderId,
				GameId: source.GameId,
				WalletId: source.WalletId,
				ExternalBetId: source.ExternalBetId),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new Common.ViewModels.GameBetInfoViewModel(
				Id: response.GameBetId,
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
				Contribute: response.Contribute,
				Jackpot: response.Jackpot,
				CreatedOn: response.CreatedOn,
				ClosedOn: response.ClosedOn,
				UpdatedOn: response.UpdatedOn,
				ResetState: response.ResetState,
				ResetOn: response.ResetOn,
				AdditionalData: JsonNode.Parse(response.AdditionalData) ?? new JsonObject { },
				ExtendedData: JsonNode.Parse(response.ExtendedData) ?? new JsonObject { });
	}

	/// <summary>
	/// Games the bet close get game bet asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("{gameBetId:guid}")]
	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetCloseGetGameBetAsync(
		[FromServices] IMediator mediator,
		Guid gameBetId,
		[FromQuery] Common.ViewModels.GameBetCloseGetGameBetViewModel source)
	{
		var response = await mediator.Send(
			request: new GameBetCloseGetGameBetRequest(
				ExternalCreatedOn: source.ExternalCreatedOn,
				GameBetId: gameBetId),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new Common.ViewModels.GameBetInfoViewModel(
				Id: response.GameBetId,
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
				Contribute: response.Contribute,
				Jackpot: response.Jackpot,
				CreatedOn: response.CreatedOn,
				ClosedOn: response.ClosedOn,
				UpdatedOn: response.UpdatedOn,
				ResetState: response.ResetState,
				ResetOn: response.ResetOn,
				AdditionalData: JsonNode.Parse(response.AdditionalData) ?? new JsonObject { },
				ExtendedData: JsonNode.Parse(response.ExtendedData) ?? new JsonObject { });
	}
}
