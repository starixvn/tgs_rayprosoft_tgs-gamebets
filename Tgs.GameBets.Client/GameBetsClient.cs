using System.Text.Json;

using System.Text.Json.Serialization;
using Tgs.GameBets.Common.ViewModels;

namespace Tgs.GameBets.Client;

internal partial class GameBetsClient(
	HttpClient httpClient) : IGameBetsClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};
}
