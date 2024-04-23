namespace Tgs.GameBets.Core.ApplicationServices;
public record GameBetSingleQuery(
	Guid Id,
	DateTimeOffset? ExternalCreatedOn);
