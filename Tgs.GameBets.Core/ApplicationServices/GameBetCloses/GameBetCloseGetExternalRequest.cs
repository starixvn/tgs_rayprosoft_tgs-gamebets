using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetCloses;
public record GameBetCloseGetExternalRequest(
	DateTimeOffset ExternalCreatedOn,
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalBetId) : IRequest<GameBetCloseInfoResponse?>;
