using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;
public record GameBetRapidTestExpireCommandRequest(
	string GameProviderCode) : IRequest;
