using Microsoft.EntityFrameworkCore;
using Tgs.GameBets;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameBets(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<RedisOptions, IServiceProvider> redisOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddGameBetsCore()
		.AddGameBetsRepositories(
			dbContextOptions: dbContextOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
