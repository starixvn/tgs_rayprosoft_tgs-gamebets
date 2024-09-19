using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Tgs.GameBets.Core.Models.GameBetMajors;
public record GameBetMajorRunningModel(
	Guid Id,
	DateTimeOffset ExternalCreatedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Turnover,
	JsonNode AdditionalData,
	JsonNode ExtendedData);