using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetCloses;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories;

internal class GameBetCloseRepository(
	TgsContext context,
	TimeProvider timeProvider) : IGameBetCloseRepository
{
	public async ValueTask AddAsync(GameBetCloseAddModel source, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var entity = new GameBetClose
		{
			Id = source.Id,
			GameBetMajorId = source.GameBetId,
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
			Turnover = source.Turnover,
			Commission = source.Commission,
			Contribute = source.Contribute,
			Jackpot = source.Jackpot,
			CreatedOn = source.CreatedOn.UtcDateTime,
			ClosedOn = source.ClosedOn.UtcDateTime,
			UpdatedOn = currentTime.UtcDateTime,
			ResetState = (int)Common.Enums.ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = source.AdditionalData.ToJsonString(),
			ExtendedData = source.ExtendedData.ToJsonString()
		};

		_ = await context.GameBetCloses.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset closedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
	=> await context.GameBetCloses
		.Where(gbc => gbc.ClosedOn >= closedOn.UtcDateTime.AddDays(-7) &&
			gbc.ClosedOn < DateTimeOffset.UtcNow.UtcDateTime &&
			gbc.GameProviderId == gameProviderId &&
			gbc.GameId == gameId &&
			gbc.WalletId == walletId &&
			gbc.ExternalBetId == externalBetId)
		.Select(gbc => new GameBetCloseInfoModel(
			gbc.Id,
			gbc.GameBetMajorId,
			gbc.GameProviderId,
			gbc.GameProviderConfigurationId,
			gbc.GameId,
			gbc.ResellerId,
			gbc.LicenseeId,
			gbc.BrandId,
			gbc.PlayerCode,
			gbc.CurrencyCode,
			gbc.WalletId,
			gbc.ExternalRoundId,
			gbc.ExternalBetId,
			gbc.ExternalCreatedOn,
			gbc.ExternalClosedOn,
			gbc.ExternalPayoffOn,
			gbc.ExternalResettleOn,
			(Common.Enums.BetState)gbc.BetState,
			(Common.Enums.BetType)gbc.BetType,
			(Common.Enums.FreeGameState)gbc.FreeGameState,
			(Common.Enums.GambleState)gbc.GambleState,
			(Common.Enums.CashoutState)gbc.CashoutState,
			(Common.Enums.ResettleState)gbc.ResettleState,
			(Common.Enums.JackpotState)gbc.JackpotState,
			gbc.CurrencyRatio,
			gbc.Bet,
			gbc.ValidBet,
			gbc.Settle,
			gbc.WinLoss,
			gbc.Turnover,
			gbc.Commission,
			gbc.Contribute,
			gbc.Jackpot,
			gbc.CreatedOn,
			gbc.ClosedOn,
			gbc.UpdatedOn,
			(Common.Enums.ResetState)gbc.ResetState,
			gbc.ResetOn,
			gbc.AdditionalData,
			gbc.ExtendedData))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<GameBetCloseInfoModel?> GetAsync(DateTimeOffset closedOn, Guid gameBetId, CancellationToken cancellationToken = default)
	=> await context.GameBetCloses
		.Where(gbc => gbc.ClosedOn >= closedOn.UtcDateTime.AddDays(-7) &&
			gbc.ClosedOn < DateTimeOffset.UtcNow.UtcDateTime &&
			gbc.GameBetMajorId == gameBetId)
		.Select(gbc => new GameBetCloseInfoModel(
			gbc.Id,
			gbc.GameBetMajorId,
			gbc.GameProviderId,
			gbc.GameProviderConfigurationId,
			gbc.GameId,
			gbc.ResellerId,
			gbc.LicenseeId,
			gbc.BrandId,
			gbc.PlayerCode,
			gbc.CurrencyCode,
			gbc.WalletId,
			gbc.ExternalRoundId,
			gbc.ExternalBetId,
			gbc.ExternalCreatedOn,
			gbc.ExternalClosedOn,
			gbc.ExternalPayoffOn,
			gbc.ExternalResettleOn,
			(Common.Enums.BetState)gbc.BetState,
			(Common.Enums.BetType)gbc.BetType,
			(Common.Enums.FreeGameState)gbc.FreeGameState,
			(Common.Enums.GambleState)gbc.GambleState,
			(Common.Enums.CashoutState)gbc.CashoutState,
			(Common.Enums.ResettleState)gbc.ResettleState,
			(Common.Enums.JackpotState)gbc.JackpotState,
			gbc.CurrencyRatio,
			gbc.Bet,
			gbc.ValidBet,
			gbc.Settle,
			gbc.WinLoss,
			gbc.Turnover,
			gbc.Commission,
			gbc.Contribute,
			gbc.Jackpot,
			gbc.CreatedOn,
			gbc.ClosedOn,
			gbc.UpdatedOn,
			(Common.Enums.ResetState)gbc.ResetState,
			gbc.ResetOn,
			gbc.AdditionalData,
			gbc.ExtendedData))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask UpdateAsync(GameBetCloseUpdateModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetCloses
			.Where(gbc => gbc.ClosedOn >= source.ClosedOn.UtcDateTime.AddDays(-7) &&
				gbc.ClosedOn < DateTimeOffset.UtcNow.UtcDateTime &&
				gbc.GameBetMajorId == source.GameBetId)
			.SingleAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

		var currentTime = timeProvider.GetUtcNow();

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
		entity.Turnover = source.Turnover;
		entity.Commission = source.Commission;
		entity.Contribute = source.Contribute;
		entity.Jackpot = source.Jackpot;
		entity.ResetState = (int)Common.Enums.ResetState.Reset;
		entity.ResetOn = currentTime.UtcDateTime;
		entity.UpdatedOn = currentTime.UtcDateTime;
		entity.AdditionalData = source.AdditionalData.ToJsonString();
		entity.ExtendedData = source.ExtendedData.ToJsonString();

		_ = context.GameBetCloses.Update(entity: entity);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
