using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBets;
public record GameBetGetIdRequest(
	DateTimeOffset ExternalCreatedOn,
	Guid Id) : IRequest<GameBetInfoResponse?>;
