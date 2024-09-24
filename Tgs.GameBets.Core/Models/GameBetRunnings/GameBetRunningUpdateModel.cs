using System.Text.Json.Nodes;

namespace Tgs.GameBets.Core.Models.GameBetRunnings;
public record GameBetRunningUpdateModel(
	Guid GameBetId,
	Common.Enums.BetType BetType,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	JsonNode AdditionalData,
	JsonNode ExtendedData);
