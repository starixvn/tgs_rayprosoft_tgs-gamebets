using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Core;
using Tgs.GameBets.Core.Models.GameBetRunnings;
using Tgs.GameBets.Entities;

namespace Tgs.GameBets.Repositories;

internal class GameBetRunningRepository(
	TgsContext context,
	TimeProvider timeProvider) : IGameBetRunningRepository
{
	public async ValueTask AddAsync(GameBetRunningAddModel source, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var entity = new GameBetRunning
		{
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
			BetType = (int)source.BetType,
			CurrencyRatio = source.CurrencyRatio,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			CreatedOn = source.CreatedOn.UtcDateTime,
			UpdatedOn = currentTime.UtcDateTime,
			AdditionalData = source.AdditionalData.ToJsonString(),
			ExtendedData = source.ExtendedData.ToJsonString()
		};

		_ = await context.GameBetRunnings.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask DeleteAsync(Guid gameBetId, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetRunnings
			.Where(gbr => gbr.GameBetMajorId == gameBetId)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (entity is not null)
		{
			_ = context.GameBetRunnings.Remove(entity: entity);

			_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}

	public async ValueTask<GameBetRunningInfoModel?> GetAsync(Guid gameBetId, CancellationToken cancellationToken = default)
	=> await context.GameBetRunnings
		.Where(gbr => gbr.GameBetMajorId == gameBetId)
		.Select(gbr => new GameBetRunningInfoModel(
			gbr.GameBetMajorId,
			gbr.GameProviderId,
			gbr.GameProviderConfigurationId,
			gbr.GameId,
			gbr.ResellerId,
			gbr.LicenseeId,
			gbr.BrandId,
			gbr.PlayerCode,
			gbr.CurrencyCode,
			gbr.WalletId,
			gbr.ExternalRoundId,
			gbr.ExternalBetId,
			gbr.ExternalCreatedOn,
			(Common.Enums.BetType)gbr.BetType,
			gbr.CurrencyRatio,
			gbr.Bet,
			gbr.ValidBet,
			gbr.CreatedOn,
			gbr.UpdatedOn,
			gbr.AdditionalData,
			gbr.ExtendedData))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask UpdateAsync(GameBetRunningUpdateModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetRunnings
			.Where(gbr => gbr.GameBetMajorId == source.GameBetId)
			.SingleAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var currentTime = timeProvider.GetUtcNow();

		entity.BetType = (int)source.BetType;
		entity.CurrencyRatio = source.CurrencyRatio;
		entity.Bet = source.Bet;
		entity.ValidBet = source.ValidBet;
		entity.UpdatedOn = currentTime.UtcDateTime;
		entity.AdditionalData = source.AdditionalData.ToJsonString();
		entity.ExtendedData = source.ExtendedData.ToJsonString();

		_ = context.GameBetRunnings.Update(entity: entity);

		_ = await context.SaveChangesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
