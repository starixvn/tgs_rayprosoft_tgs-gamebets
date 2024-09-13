using MediatR;
using Tgs.GameBets.Core.ApplicationServices.GameBetMajors;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetMajors;
public record GameBetMajorGetIdRequest(
	DateTimeOffset ExternalCreatedOn,
	Guid Id) : IRequest<GameBetMajorInfoResponse?>;
