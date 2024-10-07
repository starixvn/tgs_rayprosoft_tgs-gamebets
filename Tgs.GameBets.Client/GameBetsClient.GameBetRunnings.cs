using System.Net.Http.Json;
using Tgs.GameBets.Common.ViewModels;

namespace Tgs.GameBets.Client;

internal partial class GameBetsClient
{
	public async ValueTask GameBetRunningCommandAsync(GameBetRunningCommandViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var responseMessage = await httpClient.PostAsync(
			"/game-bet-runnings",
			requestContent,
			cancellationToken)
			.ConfigureAwait(false);

		_ = responseMessage.EnsureSuccessStatusCode();
	}

	public async ValueTask GameBetRunningDeleteAsync(Guid gameBetId, CancellationToken cancellationToken = default)
	{
		var responseMessage = await httpClient.DeleteAsync(
			$"/game-bet-runnings/{gameBetId}",
			cancellationToken)
			.ConfigureAwait(false);

		_ = responseMessage.EnsureSuccessStatusCode();
	}

	public async ValueTask<GameBetInfoViewModel?> GameBetRunningGetAsync(Guid gameBetId, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync($"/game-bet-runnings/{gameBetId}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
