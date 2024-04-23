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

	public async ValueTask<bool> GameBetFirstExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync($"/game-bet-first/exist?gameProviderId={gameProviderId}&gameId={gameId}&walletId={walletId}&externalBetId={externalBetId}&externalCreatedOn={externalCreatedOn:u}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<bool>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
