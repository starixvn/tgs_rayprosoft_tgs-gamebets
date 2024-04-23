using System.Text.Json.Nodes;

namespace Tgs.GameBets.Core.Models;
public record GameBetUpdateModel(
	Guid Id,
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalResettleOn,
	Common.Enums.BetState BetState,
	Common.Enums.BetType BetType,
	Common.Enums.FreeGameState FreeGameState,
	Common.Enums.GambleState GambleState,
	Common.Enums.CashoutState CashoutState,
	Common.Enums.ResettleState ResettleState,
	Common.Enums.JackpotState JackpotState,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	decimal Jackpot,
	JsonNode AdditionalData);
