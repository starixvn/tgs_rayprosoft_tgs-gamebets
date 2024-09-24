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
		.AddTransient<IGameBetRepository, GameBetRepository>()
		.Decorate<IGameBetRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetCacheLockedRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))
		.AddTransient<IGameBetMajorRepository, GameBetMajorRepository>()
		.Decorate<IGameBetMajorRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetMajorCacheLockedRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))
		.AddTransient<IGameBetRunningRepository, GameBetRunningRepository>()
		.AddTransient<IGameBetCloseRepository, GameBetCloseRepository>();
}
