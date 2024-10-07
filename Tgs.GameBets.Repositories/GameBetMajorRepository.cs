using MediatR;
using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetMajors;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories;

internal class GameBetMajorRepository(
	TgsContext context,
	TimeProvider timeProvider) : IGameBetMajorRepository
{
	public async ValueTask AddAsync(GameBetMajorAddModel source, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var isRunning = source.BetState is not Common.Enums.BetState.Waiting and not Common.Enums.BetState.Running;
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
			ExternalClosedOn = isRunning ? source.ExternalClosedOn.UtcDateTime : DateTimeOffset.UnixEpoch.UtcDateTime,
			ExternalPayoffOn = isRunning ? source.ExternalPayoffOn.UtcDateTime : DateTimeOffset.UnixEpoch.UtcDateTime,
			ExternalResettleOn = isRunning ? source.ExternalResettleOn.UtcDateTime : DateTimeOffset.UnixEpoch.UtcDateTime,
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
			Settle = isRunning ? source.Settle : 0.0m,
			WinLoss = isRunning ? source.WinLoss : 0.0m,
			Turnover = isRunning ? source.Turnover : 0.0m,
			Commission = isRunning ? source.Commission : 0.0m,
			Contribute = isRunning ? source.Contribute : 0.0m,
			Jackpot = isRunning ? source.Jackpot : 0.0m,
			CreatedOn = currentTime.UtcDateTime,
			ClosedOn = isRunning ? currentTime.UtcDateTime : DateTimeOffset.UnixEpoch.UtcDateTime,
			UpdatedOn = currentTime.UtcDateTime,
			AdditionalData = source.AdditionalData.ToJsonString(),
			ExtendedData = source.ExtendedData.ToJsonString(),
		};

		_ = await context.GameBetMajors.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask ClosedAsync(GameBetMajorClosedModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= source.ExternalCreatedOn.UtcDateTime.AddDays(-2) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow.UtcDateTime &&
			gbm.Id == source.Id &&
			(gbm.BetState == (int)Common.Enums.BetState.Waiting || gbm.BetState == (int)Common.Enums.BetState.Running))
			.SingleAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

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
		entity.AdditionalData = source.AdditionalData.ToJsonString();
		entity.ExtendedData = source.ExtendedData.ToJsonString();
		entity.ClosedOn = currentTime.UtcDateTime;
		entity.UpdatedOn = currentTime.UtcDateTime;

		_ = context.GameBetMajors.Update(entity: entity);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistsAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
		=> await context.GameBetMajors
		.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-2) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow.UtcDateTime &&
			gbm.GameProviderId == gameProviderId &&
			gbm.GameId == gameId &&
			gbm.WalletId == walletId &&
			gbm.ExternalBetId == externalBetId)
		.AnyAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	public async ValueTask<GameBetMajorInfoModel?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
	=> await context.GameBetMajors
		.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-2) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow.UtcDateTime &&
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
			gbm.Turnover,
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
		.Where(gbm => gbm.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-2) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow.UtcDateTime &&
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
			gbm.Turnover,
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

	public async ValueTask ReopenAsync(GameBetMajorReopenModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= source.ExternalCreatedOn.UtcDateTime.AddDays(-2) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow.UtcDateTime &&
			gbm.Id == source.Id &&
			gbm.BetState != (int)Common.Enums.BetState.Waiting &&
			gbm.BetState != (int)Common.Enums.BetState.Running)
			.SingleAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

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
		entity.AdditionalData = source.AdditionalData.ToJsonString();
		entity.ExtendedData = source.ExtendedData.ToJsonString();
		entity.UpdatedOn = currentTime.UtcDateTime;

		_ = context.GameBetMajors.Update(entity: entity);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask RunningAsync(GameBetMajorRunningModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetMajors
			.Where(gbm => gbm.ExternalCreatedOn >= source.ExternalCreatedOn.UtcDateTime.AddDays(-2) &&
			gbm.ExternalCreatedOn < DateTimeOffset.UtcNow.UtcDateTime &&
			gbm.Id == source.Id &&
			(gbm.BetState == (int)Common.Enums.BetState.Waiting || gbm.BetState == (int)Common.Enums.BetState.Running))
			.SingleAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var currentTime = timeProvider.GetUtcNow();

		entity.ExternalClosedOn = DateTimeOffset.UnixEpoch.UtcDateTime;
		entity.ExternalPayoffOn = DateTimeOffset.UnixEpoch.UtcDateTime;
		entity.ExternalResettleOn = DateTimeOffset.UnixEpoch.UtcDateTime;
		entity.BetState = (int)Common.Enums.BetState.Running;
		entity.Bet = source.Bet;
		entity.ValidBet = source.ValidBet;
		entity.Settle = 0.0m;
		entity.WinLoss = 0.0m;
		entity.Turnover = 0.0m;
		entity.Commission = 0.0m;
		entity.Contribute = 0.0m;
		entity.Jackpot = 0.0m;
		entity.AdditionalData = source.AdditionalData.ToJsonString();
		entity.ExtendedData = source.ExtendedData.ToJsonString();
		entity.UpdatedOn = currentTime.UtcDateTime;

		_ = context.GameBetMajors.Update(entity: entity);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
