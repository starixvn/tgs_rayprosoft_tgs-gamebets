using System.Net.Http.Json;

namespace Tgs.GameBets.Client;

internal partial class GameBetsClient
{
	public async ValueTask GameBetCommandAsync(Common.ViewModels.GameBetCommandViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var response = await httpClient.PostAsync(
			"/game-bet",
			requestContent,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync($"/game-bet/{id}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<Common.ViewModels.GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync($"/game-bet/{id}?externalCreatedOn={externalCreatedOn:u}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<Common.ViewModels.GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<Common.ViewModels.GameBetInfoViewModel?> GameBetGetByExternalBetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync($"/game-bet/external-bet?gameProviderId={gameProviderId}&gameId={gameId}&walletId={walletId}&externalBetId={externalBetId}&externalCreatedOn={externalCreatedOn:u}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<Common.ViewModels.GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
