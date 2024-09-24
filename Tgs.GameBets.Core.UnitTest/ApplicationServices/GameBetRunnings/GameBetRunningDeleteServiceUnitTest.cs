using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetRunnings;

public class GameBetRunningDeleteServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var gameBetId = Guid.NewGuid();

		var request = new GameBetRunningDeleteRequest(
			GameBetId: gameBetId);

		var sut = new GameBetRunningDeleteService(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(gameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
