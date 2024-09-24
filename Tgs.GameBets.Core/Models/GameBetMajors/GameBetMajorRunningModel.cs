using System.Text.Json.Nodes;

namespace Tgs.GameBets.Core.Models.GameBetMajors;
public record GameBetMajorRunningModel(
	Guid Id,
	DateTimeOffset ExternalCreatedOn,
	decimal Bet,
	decimal ValidBet,
	JsonNode AdditionalData,
	JsonNode ExtendedData);
