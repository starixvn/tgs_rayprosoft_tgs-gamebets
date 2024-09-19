using System.Text.Json;
using System.Text.Json.Nodes;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Core.Models.GameBetMajors;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetMajors;

internal class GameBetMajorCommandService(
	ILogger<GameBetMajorCommandService> logger,
	ISerialNumberGenerator serialNumberGenerator,
	IGameBetMajorRepository repository) : IRequestHandler<GameBetMajorCommandRequest, GameBetMajorInfoResponse>
{
	public async Task<GameBetMajorInfoResponse> Handle(GameBetMajorCommandRequest request, CancellationToken cancellationToken)
	{
		var gameBetMajorInfo = await repository.GetAsync(
			externalCreatedOn: request.ExternalCreatedOn,
			gameProviderId: request.GameProviderId,
			gameId: request.GameId,
			walletId: request.WalletId,
			externalBetId: request.ExternalBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var extendedData = JsonNode.Parse("{}")!;

		if (gameBetMajorInfo is null)
		{
			var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);

			await repository.AddAsync(
				source: new GameBetMajorAddModel(
					Id: id,
					GameProviderId: request.GameProviderId,
					GameProviderConfigurationId: request.GameProviderConfigurationId,
					GameId: request.GameId,
					ResellerId: request.ResellerId,
					LicenseeId: request.LicenseeId,
					BrandId: request.BrandId,
					PlayerCode: request.PlayerCode,
					CurrencyCode: request.CurrencyCode,
					WalletId: request.WalletId,
					ExternalRoundId: request.ExternalRoundId,
					ExternalBetId: request.ExternalBetId,
					ExternalCreatedOn: request.ExternalCreatedOn,
					ExternalClosedOn: request.ExternalClosedOn,
					ExternalPayoffOn: request.ExternalPayoffOn,
					ExternalResettleOn: request.ExternalResettleOn,
					BetState: request.BetState,
					BetType: request.BetType,
					FreeGameState: request.FreeGameState,
					GambleState: request.GambleState,
					CashoutState: request.CashoutState,
					ResettleState: request.ResettleState,
					JackpotState: request.JackpotState,
					CurrencyRatio: request.CurrencyRatio,
					Bet: request.Bet,
					ValidBet: request.ValidBet,
					Settle: request.Settle,
					WinLoss: request.WinLoss,
					Turnover: request.Turnover,
					Commission: request.Commission,
					Contribute: request.Contribute,
					Jackpot: request.Jackpot,
					AdditionalData: JsonNode.Parse(request.AdditionalData) ?? new JsonObject { },
					ExtendedData: extendedData),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			if (
				gameBetMajorInfo.BetState == request.BetState &&
				gameBetMajorInfo.BetType == request.BetType &&
				gameBetMajorInfo.FreeGameState == request.FreeGameState &&
				gameBetMajorInfo.GambleState == request.GambleState &&
				gameBetMajorInfo.CashoutState == request.CashoutState &&
				gameBetMajorInfo.ResettleState == request.ResettleState &&
				gameBetMajorInfo.JackpotState == request.JackpotState &&
				gameBetMajorInfo.Bet == request.Bet &&
				gameBetMajorInfo.ValidBet == request.ValidBet &&
				gameBetMajorInfo.Settle == request.Settle &&
				gameBetMajorInfo.WinLoss == request.WinLoss &&
				gameBetMajorInfo.Turnover == request.Turnover &&
				gameBetMajorInfo.Commission == request.Commission &&
				gameBetMajorInfo.Contribute == request.Contribute &&
				gameBetMajorInfo.Jackpot == request.Jackpot)
			{
				return new GameBetMajorInfoResponse(
					Id: gameBetMajorInfo.Id,
					GameProviderId: gameBetMajorInfo.GameProviderId,
					GameProviderConfigurationId: gameBetMajorInfo.GameProviderConfigurationId,
					GameId: gameBetMajorInfo.GameId,
					ResellerId: gameBetMajorInfo.ResellerId,
					LicenseeId: gameBetMajorInfo.LicenseeId,
					BrandId: gameBetMajorInfo.BrandId,
					PlayerCode: gameBetMajorInfo.PlayerCode,
					CurrencyCode: gameBetMajorInfo.CurrencyCode,
					WalletId: gameBetMajorInfo.WalletId,
					ExternalRoundId: gameBetMajorInfo.ExternalRoundId,
					ExternalBetId: gameBetMajorInfo.ExternalBetId,
					ExternalCreatedOn: gameBetMajorInfo.ExternalCreatedOn,
					ExternalClosedOn: gameBetMajorInfo.ExternalClosedOn,
					ExternalPayoffOn: gameBetMajorInfo.ExternalPayoffOn,
					ExternalResettleOn: gameBetMajorInfo.ExternalResettleOn,
					BetState: gameBetMajorInfo.BetState,
					BetType: gameBetMajorInfo.BetType,
					FreeGameState: gameBetMajorInfo.FreeGameState,
					GambleState: gameBetMajorInfo.GambleState,
					CashoutState: gameBetMajorInfo.CashoutState,
					ResettleState: gameBetMajorInfo.ResettleState,
					JackpotState: gameBetMajorInfo.JackpotState,
					CurrencyRatio: gameBetMajorInfo.CurrencyRatio,
					Bet: gameBetMajorInfo.Bet,
					ValidBet: gameBetMajorInfo.ValidBet,
					Settle: gameBetMajorInfo.Settle,
					WinLoss: gameBetMajorInfo.WinLoss,
					Turnover: gameBetMajorInfo.Turnover,
					Commission: gameBetMajorInfo.Commission,
					Contribute: gameBetMajorInfo.Contribute,
					Jackpot: gameBetMajorInfo.Jackpot,
					CreatedOn: gameBetMajorInfo.CreatedOn,
					ClosedOn: gameBetMajorInfo.ClosedOn,
					UpdatedOn: gameBetMajorInfo.UpdatedOn,
					AdditionalData: gameBetMajorInfo.AdditionalData,
					ExtendedData: gameBetMajorInfo.ExtendedData);
			}

			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					gameBetMajorInfo.Id,
					gameBetMajorInfo.GameProviderId,
					gameBetMajorInfo.GameId,
					gameBetMajorInfo.PlayerCode,
					gameBetMajorInfo.WalletId,
					gameBetMajorInfo.ExternalBetId,
					ExternalCreatedOn = gameBetMajorInfo.ExternalCreatedOn.ToString("u"),
					CurrentBetState = gameBetMajorInfo.BetState.ToString(),
					RequestBetState = request.BetState.ToString(),
					Message = "This game's betting data has changed."
				}));

			if ((gameBetMajorInfo.BetState == Common.Enums.BetState.Waiting || gameBetMajorInfo.BetState == Common.Enums.BetState.Running) &&
				(request.BetState == Common.Enums.BetState.Waiting || request.BetState == Common.Enums.BetState.Running) &&
				(gameBetMajorInfo.Bet != request.Bet || gameBetMajorInfo.ValidBet != request.ValidBet || gameBetMajorInfo.Turnover != request.Turnover))
			{
				await repository.RunningAsync(
					source: new GameBetMajorRunningModel(
						Id: gameBetMajorInfo.Id,
						ExternalCreatedOn: gameBetMajorInfo.ExternalCreatedOn,
						Bet: request.Bet,
						ValidBet: request.ValidBet,
						Turnover: request.Turnover,
						AdditionalData: JsonNode.Parse(request.AdditionalData) ?? new JsonObject { },
						ExtendedData: extendedData),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
			else
			{
				if ((gameBetMajorInfo.BetState == Common.Enums.BetState.Waiting || gameBetMajorInfo.BetState == Common.Enums.BetState.Running) &&
					request.BetState != Common.Enums.BetState.Waiting &&
					request.BetState != Common.Enums.BetState.Running)
				{
					await repository.ClosedAsync(
						source: new GameBetMajorClosedModel(
							Id: gameBetMajorInfo.Id,
							ExternalCreatedOn: gameBetMajorInfo.ExternalCreatedOn,
							ExternalClosedOn: request.ExternalClosedOn,
							ExternalPayoffOn: request.ExternalPayoffOn,
							ExternalResettleOn: request.ExternalResettleOn,
							BetState: request.BetState,
							BetType: request.BetType,
							FreeGameState: request.FreeGameState,
							GambleState: request.GambleState,
							CashoutState: request.CashoutState,
							ResettleState: request.ResettleState,
							JackpotState: request.JackpotState,
							CurrencyRatio: request.CurrencyRatio,
							Bet: request.Bet,
							ValidBet: request.ValidBet,
							Settle: request.Settle,
							WinLoss: request.WinLoss,
							Turnover: request.Turnover,
							Commission: request.Commission,
							Contribute: request.Contribute,
							Jackpot: request.Jackpot,
							AdditionalData: JsonNode.Parse(request.AdditionalData) ?? new JsonObject { },
							ExtendedData: extendedData),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				else
				{
					if (request.BetState is not Common.Enums.BetState.Waiting and not Common.Enums.BetState.Running)
					{
						if (
							gameBetMajorInfo.BetState != request.BetState ||
							gameBetMajorInfo.BetType != request.BetType ||
							gameBetMajorInfo.FreeGameState != request.FreeGameState ||
							gameBetMajorInfo.GambleState != request.GambleState ||
							gameBetMajorInfo.CashoutState != request.CashoutState ||
							gameBetMajorInfo.ResettleState != request.ResettleState ||
							gameBetMajorInfo.JackpotState != request.JackpotState ||
							gameBetMajorInfo.Bet != request.Bet ||
							gameBetMajorInfo.ValidBet != request.ValidBet ||
							gameBetMajorInfo.Settle != request.Settle ||
							gameBetMajorInfo.WinLoss != request.WinLoss ||
							gameBetMajorInfo.Turnover != request.Turnover ||
							gameBetMajorInfo.Commission != request.Commission ||
							gameBetMajorInfo.Contribute != request.Contribute ||
							gameBetMajorInfo.Jackpot != request.Jackpot)
						{
							await repository.ReopenAsync(
							source: new GameBetMajorReopenModel(
								Id: gameBetMajorInfo.Id,
								ExternalCreatedOn: gameBetMajorInfo.ExternalCreatedOn,
								ExternalClosedOn: request.ExternalClosedOn,
								ExternalPayoffOn: request.ExternalPayoffOn,
								ExternalResettleOn: request.ExternalResettleOn,
								BetState: request.BetState,
								BetType: request.BetType,
								FreeGameState: request.FreeGameState,
								GambleState: request.GambleState,
								CashoutState: request.CashoutState,
								ResettleState: request.ResettleState,
								JackpotState: request.JackpotState,
								CurrencyRatio: request.CurrencyRatio,
								Bet: request.Bet,
								ValidBet: request.ValidBet,
								Settle: request.Settle,
								WinLoss: request.WinLoss,
								Turnover: request.Turnover,
								Commission: request.Commission,
								Contribute: request.Contribute,
								Jackpot: request.Jackpot,
								AdditionalData: JsonNode.Parse(request.AdditionalData) ?? new JsonObject { },
								ExtendedData: extendedData),
							cancellationToken: cancellationToken)
							.ConfigureAwait(false);
						}
					}
				}
			}
		}

		gameBetMajorInfo = await repository.GetAsync(
			externalCreatedOn: request.ExternalCreatedOn,
			gameProviderId: request.GameProviderId,
			gameId: request.GameId,
			walletId: request.WalletId,
			externalBetId: request.ExternalBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(JsonSerializer.Serialize(new
			{
				ExternalCreatedOn = request.ExternalCreatedOn.ToString("u"),
				request.GameProviderId,
				request.GameId,
				request.WalletId,
				request.ExternalBetId,
			}), nameof(GameBetMajorInfoModel));

		return new GameBetMajorInfoResponse(
			Id: gameBetMajorInfo.Id,
			GameProviderId: gameBetMajorInfo.GameProviderId,
			GameProviderConfigurationId: gameBetMajorInfo.GameProviderConfigurationId,
			GameId: gameBetMajorInfo.GameId,
			ResellerId: gameBetMajorInfo.ResellerId,
			LicenseeId: gameBetMajorInfo.LicenseeId,
			BrandId: gameBetMajorInfo.BrandId,
			PlayerCode: gameBetMajorInfo.PlayerCode,
			CurrencyCode: gameBetMajorInfo.CurrencyCode,
			WalletId: gameBetMajorInfo.WalletId,
			ExternalRoundId: gameBetMajorInfo.ExternalRoundId,
			ExternalBetId: gameBetMajorInfo.ExternalBetId,
			ExternalCreatedOn: gameBetMajorInfo.ExternalCreatedOn,
			ExternalClosedOn: gameBetMajorInfo.ExternalClosedOn,
			ExternalPayoffOn: gameBetMajorInfo.ExternalPayoffOn,
			ExternalResettleOn: gameBetMajorInfo.ExternalResettleOn,
			BetState: gameBetMajorInfo.BetState,
			BetType: gameBetMajorInfo.BetType,
			FreeGameState: gameBetMajorInfo.FreeGameState,
			GambleState: gameBetMajorInfo.GambleState,
			CashoutState: gameBetMajorInfo.CashoutState,
			ResettleState: gameBetMajorInfo.ResettleState,
			JackpotState: gameBetMajorInfo.JackpotState,
			CurrencyRatio: gameBetMajorInfo.CurrencyRatio,
			Bet: gameBetMajorInfo.Bet,
			ValidBet: gameBetMajorInfo.ValidBet,
			Settle: gameBetMajorInfo.Settle,
			WinLoss: gameBetMajorInfo.WinLoss,
			Turnover: gameBetMajorInfo.Turnover,
			Commission: gameBetMajorInfo.Commission,
			Contribute: gameBetMajorInfo.Contribute,
			Jackpot: gameBetMajorInfo.Jackpot,
			CreatedOn: gameBetMajorInfo.CreatedOn,
			ClosedOn: gameBetMajorInfo.ClosedOn,
			UpdatedOn: gameBetMajorInfo.UpdatedOn,
			AdditionalData: gameBetMajorInfo.AdditionalData,
			ExtendedData: gameBetMajorInfo.ExtendedData);
	}
}
