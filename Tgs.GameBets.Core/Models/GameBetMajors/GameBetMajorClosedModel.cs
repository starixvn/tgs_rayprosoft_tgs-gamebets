using System.Text.Json.Nodes;

namespace Tgs.GameBets.Core.Models.GameBetMajors;
public record GameBetMajorClosedModel(
	Guid Id,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	DateTimeOffset ExternalPayoffOn,
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
	decimal WinLoss,
	decimal Turnover,
	decimal Commission,
	decimal Contribute,
	decimal Jackpot,
	JsonNode AdditionalData,
	JsonNode ExtendedData);
