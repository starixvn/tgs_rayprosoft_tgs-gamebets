namespace Tgs.GameBets.Core.Models;
public record GameBetPrimaryKeyModel(
	Guid Id,
	Guid GameProviderId,
	int GameId,
	string PlayerCode,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn);
