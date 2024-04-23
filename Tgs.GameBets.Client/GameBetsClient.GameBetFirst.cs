using System.Net.Http.Json;

namespace Tgs.GameBets.Client;

internal partial class GameBetsClient
{
	public async ValueTask GameBetFirstCommandAsync(Common.ViewModels.GameBetFirstCommandViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var response = await httpClient.PostAsync(
			"/game-bet-first",
			requestContent,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}
}
