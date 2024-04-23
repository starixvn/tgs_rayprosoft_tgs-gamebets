using Microsoft.AspNetCore.Mvc;
using Tgs.GameBets.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Controllers;

[Route("game-bet")]
[ApiController]
public class GameBetController : ControllerBase
{
	/// <summary>
	/// Executes the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost(Name = "GameBet_Command")]
	public ValueTask ExecuteAsync(
		[FromServices] ICommandService<GameBetCommand> service,
		[FromBody] Common.ViewModels.GameBetCommandViewModel source)
		=> service.ExecuteAsync(
			command: new GameBetCommand(
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
				AdditionalData: source.AdditionalData),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Gets the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="externalBetId">The external bet identifier.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("{externalBetId}/external-bet", Name = "GameBet_GetByExternalBet")]
	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GetAsync(
		[FromServices] IGetService<GameBetSingleByExternalBetQuery, GameBetReslut?> service,
		string externalBetId,
		[FromQuery] Common.ViewModels.GameBetByExternalBetViewMode source)
	{
		var info = await service.GetAsync(
			query: new GameBetSingleByExternalBetQuery(
				GameProviderId: source.GameProviderId,
				GameId: source.GameId,
				WalletId: source.WalletId,
				ExternalBetId: externalBetId,
				ExternalCreatedOn: source.ExternalCreatedOn),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return info is null
			? null
			: new Common.ViewModels.GameBetInfoViewModel(
				Id: info.Id,
				GameProviderId: info.GameProviderId,
				GameProviderConfigurationId: info.GameProviderConfigurationId,
				GameId: info.GameId,
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				CurrencyCode: info.CurrencyCode,
				WalletId: info.WalletId,
				ExternalRoundId: info.ExternalRoundId,
				ExternalBetId: info.ExternalBetId,
				ExternalCreatedOn: info.ExternalCreatedOn,
				ExternalClosedOn: info.ExternalClosedOn,
				ExternalPayoffOn: info.ExternalPayoffOn,
				ExternalResettleOn: info.ExternalResettleOn,
				BetState: info.BetState,
				BetType: info.BetType,
				FreeGameState: info.FreeGameState,
				GambleState: info.GambleState,
				CashoutState: info.CashoutState,
				ResettleState: info.ResettleState,
				JackpotState: info.JackpotState,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				Commission: info.Commission,
				Turnover: info.Turnover,
				WinLoss: info.WinLoss,
				Jackpot: info.Jackpot,
				CreatedOn: info.CreatedOn,
				ClosedOn: info.ClosedOn,
				UpdatedOn: info.UpdatedOn,
				ResetState: info.ResetState,
				ResetOn: info.ResetOn,
				AdditionalData: info.AdditionalData);
	}

	/// <summary>
	/// Gets the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="id">The identifier.</param>
	/// <param name="externalCreatedOn">The external created on.</param>
	/// <returns></returns>
	[HttpGet("{id:guid}", Name = "GameBet_Get")]
	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GetAsync(
		[FromServices] IGetService<GameBetSingleQuery, GameBetReslut?> service,
		Guid id,
		[FromQuery] DateTimeOffset? externalCreatedOn)
	{
		var info = await service.GetAsync(
			query: new GameBetSingleQuery(
				Id: id,
				ExternalCreatedOn: externalCreatedOn),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return info is null
			? null
			: new Common.ViewModels.GameBetInfoViewModel(
				Id: info.Id,
				GameProviderId: info.GameProviderId,
				GameProviderConfigurationId: info.GameProviderConfigurationId,
				GameId: info.GameId,
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				CurrencyCode: info.CurrencyCode,
				WalletId: info.WalletId,
				ExternalRoundId: info.ExternalRoundId,
				ExternalBetId: info.ExternalBetId,
				ExternalCreatedOn: info.ExternalCreatedOn,
				ExternalClosedOn: info.ExternalClosedOn,
				ExternalPayoffOn: info.ExternalPayoffOn,
				ExternalResettleOn: info.ExternalResettleOn,
				BetState: info.BetState,
				BetType: info.BetType,
				FreeGameState: info.FreeGameState,
				GambleState: info.GambleState,
				CashoutState: info.CashoutState,
				ResettleState: info.ResettleState,
				JackpotState: info.JackpotState,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				Commission: info.Commission,
				Turnover: info.Turnover,
				WinLoss: info.WinLoss,
				Jackpot: info.Jackpot,
				CreatedOn: info.CreatedOn,
				ClosedOn: info.ClosedOn,
				UpdatedOn: info.UpdatedOn,
				ResetState: info.ResetState,
				ResetOn: info.ResetOn,
				AdditionalData: info.AdditionalData);
	}
}
