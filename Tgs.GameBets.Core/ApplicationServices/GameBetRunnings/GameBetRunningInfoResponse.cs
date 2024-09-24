using System.Text.Json.Nodes;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;
public record GameBetRunningInfoResponse(
	Guid GameBetId,
	Guid GameProviderId,
	Guid GameProviderConfigurationId,
	int GameId,
	Guid ResellerId,
	Guid LicenseeId,
	Guid BrandId,
	string PlayerCode,
	string CurrencyCode,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	Common.Enums.BetType BetType,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	DateTimeOffset CreatedOn,
	DateTimeOffset UpdatedOn,
	string AdditionalData,
	string ExtendedData);