namespace Tgs.GameBets.Core.ApplicationServices;
public record GameBetSingleByExternalBetQuery(
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn);
