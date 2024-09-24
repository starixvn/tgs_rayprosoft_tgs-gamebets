using System.Text.Json.Nodes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;

namespace Tgs.GameBets.Controllers;

[Route("game-bet-runnings")]
[ApiController]
public class GameBetRunningsController : ControllerBase
{
	/// <summary>
	/// Games the bet running command asynchronous.
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
	///			"betType": "Normal",
	///			"currencyRatio": 1,
	///			"bet": 0.8,
	///			"validBet": 0.8,
	///			"createdOn": "2024-09-12T09:56:05+08:00",
	///			"additionalData": {}
	///		}
	/// </remarks>
	[HttpPost()]
	public async ValueTask GameBetRunningCommandAsync(
		[FromServices] IMediator mediator,
		[FromBody] Common.ViewModels.GameBetRunningCommandViewModel source)
		=> await mediator.Send(
			request: new GameBetRunningCommandRequest(
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
				BetType: source.BetType,
				CurrencyRatio: source.CurrencyRatio,
				Bet: source.Bet,
				ValidBet: source.ValidBet,
				CreatedOn: source.CreatedOn,
				AdditionalData: source.AdditionalData,
				ExtendedData: source.ExtendedData
				),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

	/// <summary>
	/// Games the bet running get asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <returns></returns>
	[HttpGet("{gameBetId:guid}")]
	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetRunningGetAsync(
		[FromServices] IMediator mediator,
		Guid gameBetId)
	{
		var response = await mediator.Send(
			request: new GameBetRunningGetRequest(
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
				ExternalClosedOn: DateTimeOffset.UnixEpoch,
				ExternalPayoffOn: DateTimeOffset.UnixEpoch,
				ExternalResettleOn: DateTimeOffset.UnixEpoch,
				BetState: Common.Enums.BetState.Running,
				BetType: response.BetType,
				FreeGameState: Common.Enums.FreeGameState.None,
				GambleState: Common.Enums.GambleState.None,
				CashoutState: Common.Enums.CashoutState.None,
				ResettleState: Common.Enums.ResettleState.None,
				JackpotState: Common.Enums.JackpotState.None,
				CurrencyRatio: response.CurrencyRatio,
				Bet: response.Bet,
				ValidBet: response.ValidBet,
				Settle: 0.0m,
				WinLoss: 0.0m,
				Turnover: 0.0m,
				Commission: 0.0m,
				Contribute: 0.0m,
				Jackpot: 0.0m,
				CreatedOn: response.CreatedOn,
				ClosedOn: DateTimeOffset.UnixEpoch,
				UpdatedOn: response.UpdatedOn,
				ResetState: Common.Enums.ResetState.None,
				ResetOn: DateTimeOffset.UnixEpoch,
				AdditionalData: JsonNode.Parse(response.AdditionalData) ?? new JsonObject { },
				ExtendedData: JsonNode.Parse(response.ExtendedData) ?? new JsonObject { });
	}

	/// <summary>
	/// Games the bet running delete asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <returns></returns>
	[HttpDelete("{gameBetId:guid}")]
	public async ValueTask GameBetRunningDeleteAsync(
		[FromServices] IMediator mediator,
		Guid gameBetId)
		=> await mediator.Send(
			request: new GameBetRunningDeleteRequest(
				GameBetId: gameBetId),
			cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);
}
