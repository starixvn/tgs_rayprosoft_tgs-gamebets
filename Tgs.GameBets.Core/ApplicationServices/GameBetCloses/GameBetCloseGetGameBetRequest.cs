using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetCloses;
public record GameBetCloseGetGameBetRequest(
	DateTimeOffset ExternalCreatedOn,
	Guid GameBetId) : IRequest<GameBetCloseInfoResponse?>;
