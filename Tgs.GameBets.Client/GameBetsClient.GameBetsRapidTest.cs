using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameBets.Common.ViewModels;

namespace Tgs.GameBets.Client;
internal partial class GameBetsClient
{
	public async ValueTask<bool> GameBetRapidTestCommandAsync(GameBetRapidTestCommandViewModel source, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, $"/game-bet-rapid-test");
		var builder = new UriBuilder(path);
		var requestContent = JsonContent.Create(inputValue: source, options: _SerializerOptions);
		var response = await httpClient.PostAsync(
			builder.ToString(),
			requestContent,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<bool>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}
	public async ValueTask GameBetRapidTestExpireAsync(string gameProviderCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PutAsync(
			$"/game-bet-rapid-test/{gameProviderCode}/expire",
			null,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}
	public async ValueTask GameBetRapidTestInitializationAsync(string gameProviderCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PutAsync(
			$"/game-bet-rapid-test/{gameProviderCode}/init",
			null,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}
}
