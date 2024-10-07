using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Core.ApplicationServices.GameBetRapidTests;

namespace Tgs.GameBets.Core.UnitTest.ApplicationServices.GameBetRapidTests;

public class GameBetRapidTestExpireCommandServiceUnitTest
{
	[Fact]
	public async Task Locked_Is_True()
	{
		var faceLogger = NullLogger<GameBetRapidTestExpireCommandService>.Instance;
		var fakerGameBetRapidTestExpireLockedRepository = Substitute.For<IGameBetRapidTestExpireLockedRepository>();
		var fakeGameBetRapidTestRepository = Substitute.For<IGameBetRapidTestRepository>();

		var request = new GameBetRapidTestExpireCommandRequest(
			GameProviderCode: "KY");

		_ = fakerGameBetRapidTestExpireLockedRepository.LockedAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			application: Arg.Is(typeof(GameBetRapidTestExpireCommandService).Name),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new GameBetRapidTestExpireCommandService(
			faceLogger,
			fakerGameBetRapidTestExpireLockedRepository,
			fakeGameBetRapidTestRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeGameBetRapidTestRepository
			.Received(1)
			.SetExpireOnAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerGameBetRapidTestExpireLockedRepository
			.Received(1)
			.UnlockAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			application: Arg.Is(typeof(GameBetRapidTestExpireCommandService).Name));
	}

	[Fact]
	public async Task Locked_Is_False()
	{
		var faceLogger = NullLogger<GameBetRapidTestExpireCommandService>.Instance;
		var fakerGameBetRapidTestExpireLockedRepository = Substitute.For<IGameBetRapidTestExpireLockedRepository>();
		var fakeGameBetRapidTestRepository = Substitute.For<IGameBetRapidTestRepository>();

		var request = new GameBetRapidTestExpireCommandRequest(
			GameProviderCode: "KY");

		_ = fakerGameBetRapidTestExpireLockedRepository.LockedAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			application: Arg.Is(typeof(GameBetRapidTestExpireCommandService).Name),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var sut = new GameBetRapidTestExpireCommandService(
			faceLogger,
			fakerGameBetRapidTestExpireLockedRepository,
			fakeGameBetRapidTestRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeGameBetRapidTestRepository
			.Received(0)
			.SetExpireOnAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerGameBetRapidTestExpireLockedRepository
			.Received(0)
			.UnlockAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			application: Arg.Is(typeof(GameBetRapidTestExpireCommandService).Name));
	}

	[Fact]
	public async Task SetExpireOn_Exception()
	{
		var faceLogger = NullLogger<GameBetRapidTestExpireCommandService>.Instance;
		var fakerGameBetRapidTestExpireLockedRepository = Substitute.For<IGameBetRapidTestExpireLockedRepository>();
		var fakeGameBetRapidTestRepository = Substitute.For<IGameBetRapidTestRepository>();

		var request = new GameBetRapidTestExpireCommandRequest(
			GameProviderCode: "KY");

		_ = fakerGameBetRapidTestExpireLockedRepository.LockedAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			application: Arg.Is(typeof(GameBetRapidTestExpireCommandService).Name),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameBetRapidTestRepository
			.SetExpireOnAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException<long>(new Exception()));

		var sut = new GameBetRapidTestExpireCommandService(
			faceLogger,
			fakerGameBetRapidTestExpireLockedRepository,
			fakeGameBetRapidTestRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<Exception>(() => sut.Handle(request, token));

		_ = fakerGameBetRapidTestExpireLockedRepository
			.Received(1)
			.UnlockAsync(
			gameProviderCode: Arg.Is(request.GameProviderCode),
			application: Arg.Is(typeof(GameBetRapidTestExpireCommandService).Name));
	}
}
