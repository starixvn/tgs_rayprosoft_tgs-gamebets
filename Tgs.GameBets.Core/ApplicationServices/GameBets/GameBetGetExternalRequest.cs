using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBets;
public record GameBetGetExternalRequest(
	DateTimeOffset ExternalCreatedOn,
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalBetId) : IRequest<GameBetInfoResponse?>;
