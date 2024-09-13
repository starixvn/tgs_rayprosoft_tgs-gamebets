using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetMajors;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories;

internal class GameBetMajorRepository(
	TgsContext context) : IGameBetMajorRepository
{
	public async ValueTask AddAsync(GameBetMajorAddModel source, CancellationToken cancellationToken = default)
	{
		var entity = new GameBetMajor
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
			WinLoss = source.WinLoss,
			Commission = source.Commission,
			Contribute = source.Contribute,
			Jackpot = source.Jackpot,
			CreatedOn = DateTime.UtcNow,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			AdditionalData = source.AdditionalData.ToJsonString(),
			ExtendedData = source.ExtendedData.ToJsonString(),
		};

		_ = await context.GameBetMajors.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
	=> await context.GameBetMajors
		.AnyAsync(gb => gb.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-3) &&
			gb.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gb.GameProviderId == gameProviderId &&
			gb.GameId == gameId &&
			gb.WalletId == walletId &&
			gb.ExternalBetId == externalBetId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<GameBetMajorInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
	=> await context.GameBetMajors
		.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-3) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.GameProviderId == gameProviderId &&
			gbm.GameId == gameId &&
			gbm.WalletId == walletId &&
			gbm.ExternalBetId == externalBetId)
		.Select(gbm => new GameBetMajorInfoModel(
			gbm.Id,
			gbm.GameProviderId,
			gbm.GameProviderConfigurationId,
			gbm.GameId,
			gbm.ResellerId,
			gbm.LicenseeId,
			gbm.BrandId,
			gbm.PlayerCode,
			gbm.CurrencyCode,
			gbm.WalletId,
			gbm.ExternalRoundId,
			gbm.ExternalBetId,
			gbm.ExternalCreatedOn,
			gbm.ExternalClosedOn,
			gbm.ExternalPayoffOn,
			gbm.ExternalResettleOn,
			(Common.Enums.BetState)gbm.BetState,
			(Common.Enums.BetType)gbm.BetType,
			(Common.Enums.FreeGameState)gbm.FreeGameState,
			(Common.Enums.GambleState)gbm.GambleState,
			(Common.Enums.CashoutState)gbm.CashoutState,
			(Common.Enums.ResettleState)gbm.ResettleState,
			(Common.Enums.JackpotState)gbm.JackpotState,
			gbm.CurrencyRatio,
			gbm.Bet,
			gbm.ValidBet,
			gbm.Settle,
			gbm.WinLoss,
			gbm.Commission,
			gbm.Contribute,
			gbm.Jackpot,
			gbm.CreatedOn,
			gbm.ClosedOn,
			gbm.UpdatedOn,
			gbm.AdditionalData,
			gbm.ExtendedData))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<GameBetMajorInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default)
	=> await context.GameBetMajors
		.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-3) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.Id == id)
		.Select(gbm => new GameBetMajorInfoModel(
			gbm.Id,
			gbm.GameProviderId,
			gbm.GameProviderConfigurationId,
			gbm.GameId,
			gbm.ResellerId,
			gbm.LicenseeId,
			gbm.BrandId,
			gbm.PlayerCode,
			gbm.CurrencyCode,
			gbm.WalletId,
			gbm.ExternalRoundId,
			gbm.ExternalBetId,
			gbm.ExternalCreatedOn,
			gbm.ExternalClosedOn,
			gbm.ExternalPayoffOn,
			gbm.ExternalResettleOn,
			(Common.Enums.BetState)gbm.BetState,
			(Common.Enums.BetType)gbm.BetType,
			(Common.Enums.FreeGameState)gbm.FreeGameState,
			(Common.Enums.GambleState)gbm.GambleState,
			(Common.Enums.CashoutState)gbm.CashoutState,
			(Common.Enums.ResettleState)gbm.ResettleState,
			(Common.Enums.JackpotState)gbm.JackpotState,
			gbm.CurrencyRatio,
			gbm.Bet,
			gbm.ValidBet,
			gbm.Settle,
			gbm.WinLoss,
			gbm.Commission,
			gbm.Contribute,
			gbm.Jackpot,
			gbm.CreatedOn,
			gbm.ClosedOn,
			gbm.UpdatedOn,
			gbm.AdditionalData,
			gbm.ExtendedData))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask UpdateAsync(GameBetMajorUpdateModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= source.ExternalCreatedOn.AddDays(-3) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow &&
			gbm.Id == source.Id)
			.SingleAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		entity.ExternalClosedOn = source.ExternalClosedOn.UtcDateTime;
		entity.ExternalPayoffOn = source.ExternalPayoffOn.UtcDateTime;
		entity.ExternalResettleOn = source.ExternalResettleOn.UtcDateTime;
		entity.BetState = (int)source.BetState;
		entity.BetType = (int)source.BetType;
		entity.FreeGameState = (int)source.FreeGameState;
		entity.GambleState = (int)source.GambleState;
		entity.CashoutState = (int)source.CashoutState;
		entity.ResettleState = (int)source.ResettleState;
		entity.JackpotState = (int)source.JackpotState;
		entity.CurrencyRatio = source.CurrencyRatio;
		entity.Bet = source.Bet;
		entity.ValidBet = source.ValidBet;
		entity.Settle = source.Settle;
		entity.WinLoss = source.WinLoss;
		entity.Commission = source.Commission;
		entity.Contribute = source.Contribute;
		entity.Jackpot = source.Jackpot;
		entity.AdditionalData = source.AdditionalData.ToJsonString();
		entity.ExtendedData = source.ExtendedData.ToJsonString();
		entity.UpdatedOn = DateTime.UtcNow;

		_ = context.GameBetMajors.Update(entity: entity);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
