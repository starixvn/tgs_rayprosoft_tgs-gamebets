using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;
public record GameBetRapidTestInitializationCommandRequest(
	string GameProviderCode) : IRequest;
