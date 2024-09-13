using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetMajors;
public record GameBetMajorGetExternalRequest(
	DateTimeOffset ExternalCreatedOn,
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalBetId) : IRequest<GameBetMajorInfoResponse?>;
