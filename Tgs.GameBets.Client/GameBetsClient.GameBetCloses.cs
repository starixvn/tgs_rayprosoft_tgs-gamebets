using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameBets.Common.Enums;
using Tgs.GameBets.Common.ViewModels;

namespace Tgs.GameBets.Client;

internal partial class GameBetsClient
{
	public async ValueTask<GameBetCommandResult> GameBetClosesCommandAsync(GameBetCloseCommandViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		return await httpClient.PostAsync(
			"/game-bet-closes",
			requestContent,
			cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<GameBetCommandResult>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<GameBetInfoViewModel?> GameBetClosesGetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync($"/game-bet-closes/external-bet?gameProviderId={gameProviderId}&gameId={gameId}&walletId={walletId}&externalBetId={externalBetId}&externalCreatedOn={externalCreatedOn:u}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<GameBetInfoViewModel?> GameBetClosesGetAsync(DateTimeOffset externalCreatedOn, Guid gameBetId, CancellationToken cancellationToken = default)
			=> await httpClient.GetAsync($"/game-bet-closes/{gameBetId}?externalCreatedOn={externalCreatedOn:u}", cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<GameBetInfoViewModel>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);
}
