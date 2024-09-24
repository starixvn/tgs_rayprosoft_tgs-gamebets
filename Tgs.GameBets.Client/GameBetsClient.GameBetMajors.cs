using System.Net.Http.Json;
using System.Text.Json;
using Tgs.GameBets.Common.ViewModels;

namespace Tgs.GameBets.Client;

internal partial class GameBetsClient
{
	public async ValueTask<GameBetInfoViewModel> GameBetMajorCommandAsync(GameBetMajorCommandViewModel source, CancellationToken cancellationToken = default)
		=> await httpClient.PostAsJsonAsync(
			requestUri: "/game-bet-majors",
			value: source,
			options: _SerializerOptions,
			cancellationToken: cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(JsonSerializer.Serialize(new
			{
				ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
				source.GameProviderId,
				source.GameId,
				source.WalletId,
				source.ExternalBetId,
			}), nameof(GameBetInfoViewModel));

	public async ValueTask<GameBetInfoViewModel?> GameBetMajorGetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync(
			requestUri: $"/game-bet-majors/external-bet?gameProviderId={gameProviderId}&gameId={gameId}&walletId={walletId}&externalBetId={externalBetId}&externalCreatedOn={externalCreatedOn:u}",
			cancellationToken: cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<GameBetInfoViewModel?> GameBetMajorGetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync(
			 requestUri: $"/game-bet-majors/{id}?externalCreatedOn={externalCreatedOn:u}",
			 cancellationToken: cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
