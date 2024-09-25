using System.Text.Json;
using MediatR;
using OpenTelemetry.Trace;

namespace Tgs.GameBets;

public class TelemetryBehavior<TRequest, TResponse>(TracerProvider tracerProvider) : IPipelineBehavior<TRequest, TResponse>
{
	private readonly Tracer _tracer = tracerProvider.GetTracer("MediatR");
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		using var span = _tracer.StartSpan("HandleRequest");
		// Add any attributes or events as needed
		return await next().ConfigureAwait(false);
	}
}
