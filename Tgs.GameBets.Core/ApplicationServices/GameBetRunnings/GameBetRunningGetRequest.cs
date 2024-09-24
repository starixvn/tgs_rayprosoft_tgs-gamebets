using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;
public record GameBetRunningGetRequest(
	Guid GameBetId) : IRequest<GameBetRunningInfoResponse?>;
