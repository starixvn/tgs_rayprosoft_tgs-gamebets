using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBets;

using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories;

internal class GameBetRepository(
	TgsContext context) : IGameBetRepository
{
	public async ValueTask AddAsync(GameBetAddModel source, CancellationToken cancellationToken = default)
	{
		var entity = new GameBet
		{
			Id = source.Id,
			GameProviderId = source.GameProviderId,
			GameProviderConfigurationId = source.GameProviderConfigurationId,
			GameId = source.GameId,
			ResellerId = source.ResellerId,
			LicenseeId = source.LicenseeId,
			BrandId = source.BrandId,
			PlayerCode = source.PlayerCode,
			CurrencyCode = source.CurrencyCode,
			WalletId = source.WalletId,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalCreatedOn = source.ExternalCreatedOn.UtcDateTime,
			ExternalClosedOn = source.ExternalClosedOn.UtcDateTime,
			ExternalPayoffOn = source.ExternalPayoffOn.UtcDateTime,
			ExternalResettleOn = source.ExternalResettleOn.UtcDateTime,
			BetState = (int)source.BetState,
			BetType = (int)source.BetType,
			FreeGameState = (int)source.FreeGameState,
			GambleState = (int)source.GambleState,
			CashoutState = (int)source.CashoutState,
			ResettleState = (int)source.ResettleState,
			JackpotState = (int)source.JackpotState,
			CurrencyRatio = source.CurrencyRatio,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = source.Settle,
			Commission = source.Commission,
			Turnover = source.Turnover,
			WinLoss = source.WinLoss,
			Jackpot = source.Jackpot,
			CreatedOn = DateTime.UtcNow,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = source.AdditionalData.ToJsonString()
		};

		_ = await context.GameBets.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<GameBetInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
	=> await context.GameBets
		.Where(gb => gb.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gb.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gb.GameProviderId == gameProviderId &&
			gb.GameId == gameId &&
			gb.WalletId == walletId &&
			gb.ExternalBetId == externalBetId)
		.Select(gb => new GameBetInfoModel(
			gb.Id,
			gb.GameProviderId,
			gb.GameProviderConfigurationId,
			gb.GameId,
			gb.ResellerId,
			gb.LicenseeId,
			gb.BrandId,
			gb.PlayerCode,
			gb.CurrencyCode,
			gb.WalletId,
			gb.ExternalRoundId,
			gb.ExternalBetId,
			gb.ExternalCreatedOn,
			gb.ExternalClosedOn,
			gb.ExternalPayoffOn,
			gb.ExternalResettleOn,
			(Common.Enums.BetState)gb.BetState,
			(Common.Enums.BetType)gb.BetType,
			(Common.Enums.FreeGameState)gb.FreeGameState,
			(Common.Enums.GambleState)gb.GambleState,
			(Common.Enums.CashoutState)gb.CashoutState,
			(Common.Enums.ResettleState)gb.ResettleState,
			(Common.Enums.JackpotState)gb.JackpotState,
			gb.CurrencyRatio,
			gb.Bet,
			gb.ValidBet,
			gb.Settle,
			gb.Commission,
			gb.Turnover,
			gb.WinLoss,
			gb.Jackpot,
			gb.CreatedOn,
			gb.ClosedOn,
			gb.UpdatedOn,
			(Common.Enums.ResetState)gb.ResetState,
			gb.ResetOn,
			gb.AdditionalData))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<GameBetInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default)
	=> await context.GameBets
		.Where(gb => gb.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gb.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gb.Id == id)
		.Select(gb => new GameBetInfoModel(
			gb.Id,
			gb.GameProviderId,
			gb.GameProviderConfigurationId,
			gb.GameId,
			gb.ResellerId,
			gb.LicenseeId,
			gb.BrandId,
			gb.PlayerCode,
			gb.CurrencyCode,
			gb.WalletId,
			gb.ExternalRoundId,
			gb.ExternalBetId,
			gb.ExternalCreatedOn,
			gb.ExternalClosedOn,
			gb.ExternalPayoffOn,
			gb.ExternalResettleOn,
			(Common.Enums.BetState)gb.BetState,
			(Common.Enums.BetType)gb.BetType,
			(Common.Enums.FreeGameState)gb.FreeGameState,
			(Common.Enums.GambleState)gb.GambleState,
			(Common.Enums.CashoutState)gb.CashoutState,
			(Common.Enums.ResettleState)gb.ResettleState,
			(Common.Enums.JackpotState)gb.JackpotState,
			gb.CurrencyRatio,
			gb.Bet,
			gb.ValidBet,
			gb.Settle,
			gb.Commission,
			gb.Turnover,
			gb.WinLoss,
			gb.Jackpot,
			gb.CreatedOn,
			gb.ClosedOn,
			gb.UpdatedOn,
			(Common.Enums.ResetState)gb.ResetState,
			gb.ResetOn,
			gb.AdditionalData))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask UpdateAsync(GameBetUpdateModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBets
			.Where(gb => gb.ExternalCreatedOn >= source.ExternalCreatedOn.UtcDateTime.AddDays(-7) &&
				gb.ExternalCreatedOn < DateTimeOffset.UtcNow &&
				gb.Id == source.Id)
			.SingleAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

		entity.ExternalClosedOn = source.ExternalClosedOn.UtcDateTime;
		entity.ExternalPayoffOn = source.ExternalPayoffOn.UtcDateTime;
		entity.ExternalResettleOn = source.ExternalResettleOn.UtcDateTime;
		entity.BetState = (int)source.BetState;
		entity.BetType = (int)source.BetType;
		entity.FreeGameState = (int)source.FreeGameState;
		entity.GambleState = (int)source.GambleState;
		entity.CashoutState = (int)source.CashoutState;
		entity.ResettleState = (int)source.ResettleState;
		entity.CurrencyRatio = source.CurrencyRatio;
		entity.Bet = source.Bet;
		entity.ValidBet = source.ValidBet;
		entity.Settle = source.Settle;
		entity.Commission = source.Commission;
		entity.Turnover = source.Turnover;
		entity.WinLoss = source.WinLoss;
		entity.ResetState = (int)Common.Enums.ResetState.Reset;
		entity.ResetOn = DateTime.UtcNow;
		entity.UpdatedOn = DateTime.UtcNow;
		entity.AdditionalData = source.AdditionalData.ToJsonString();

		_ = context.GameBets.Update(entity: entity);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
