using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetRapidTests;

public class GameBetRapidTestInitializationCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRapidTestRepository = Substitute.For<IGameBetRapidTestRepository>();

		var request = new GameBetRapidTestInitializationCommandRequest(
			GameProviderCode: "KY");

		var sut = new GameBetRapidTestInitializationCommandService(
			fakeGameBetRapidTestRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeGameBetRapidTestRepository
			.Received(1)
			.InitializationAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
