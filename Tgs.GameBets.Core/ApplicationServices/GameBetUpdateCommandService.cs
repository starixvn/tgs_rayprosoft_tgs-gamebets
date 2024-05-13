using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Core.ApplicationServices;

internal class GameBetUpdateCommandService(
	ILogger<GameBetUpdateCommandService> logger,
	IGameBetRepository repository,
	IMessageSender messageSender) : ICommandService<GameBetUpdateCommand>
{
	public async ValueTask ExecuteAsync(GameBetUpdateCommand command, CancellationToken cancellationToken = default)
	{
		await repository.UpdateAsync(
			source: new Models.GameBetUpdateModel(
				Id: command.GameBetId,
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				WalletId: command.WalletId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalResettleOn: command.ExternalResettleOn,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				Jackpot: command.Jackpot,
				AdditionalData: command.AdditionalData),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var data = new Common.Protos.GameBetPublish
		{
			GameBetId = command.GameBetId,
			GameProviderId = command.GameProviderId,
			GameId = command.GameId,
			ResellerId = command.ResellerId,
			LicenseeId = command.LicenseeId,
			BrandId = command.BrandId,
			PlayerCode = command.PlayerCode,
			CurrencyCode = command.CurrencyCode,
			WalletId = command.WalletId,
			ExternalRoundId = command.ExternalRoundId,
			ExternalBetId = command.ExternalBetId,
			ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp()
		};

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This game bet is update and published.({publish})",
			DateTime.Now.ToString("o"),
			Common.GameBetsNatsConst.GameBetUpdatePublish,
			new
			{
				GameBetId = data.GameBetId.ToGuid(),
				GameProviderId = data.GameProviderId.ToGuid(),
				data.GameId,
				ResellerId = data.ResellerId.ToGuid(),
				LicenseeId = data.LicenseeId.ToGuid(),
				BrandId = data.BrandId.ToGuid(),
				data.PlayerCode,
				data.CurrencyCode,
				WalletId = data.WalletId.ToGuid(),
				data.ExternalRoundId,
				data.ExternalBetId,
				ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset()
			});

		await messageSender.PublishAsync(
			subject: Common.GameBetsNatsConst.GameBetUpdatePublish,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
