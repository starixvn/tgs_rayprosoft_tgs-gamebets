using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.GameBets.Core;
using Tgs.GameBets.Entities;
using Tgs.GameBets.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameBetsRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)

		.AddTransient<IGameBetMajorRepository, GameBetMajorRepository>()
		.Decorate<IGameBetMajorRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetMajorCacheLockedRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))
		.Decorate<IGameBetMajorRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetMajorCacheGetRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))

		.AddTransient<IGameBetRunningRepository, GameBetRunningRepository>()
		.Decorate<IGameBetRunningRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetRunningCacheLockedRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))

		.AddTransient<IGameBetCloseRepository, GameBetCloseRepository>()
		.Decorate<IGameBetCloseRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetCloseCacheLockedRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))
		.Decorate<IGameBetCloseRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetCloseCacheGetRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)));
}
