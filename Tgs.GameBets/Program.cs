using System.Net.Mime;
using System.Reflection;
using System.Text.Json.Serialization;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using Tgs.GameBets;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
			fluentOptions.UseMessagePack();
		});
	});

// Add services to the container.
builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	foreach (var xmlFileName in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
	{
		c.IncludeXmlComments(xmlFileName);
	}
});

builder.Services
	.AddOptions<RedisOptions>()
	.Configure((RedisOptions options, IServiceProvider sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.ConnectionString = configuration.GetConnectionString("Redis")!;
	})
	.Services
	.AddSingleton<RedisConnectionManager>()
	.AddGameBetsCore()
	.AddGameBetsRepositories(
	(IServiceProvider sp, DbContextOptionsBuilder dbBuilder) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		_ = dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"),
			builder => builder.EnableRetryOnFailure(
				maxRetryCount: 5,
				maxRetryDelay: TimeSpan.FromSeconds(10),
				errorCodesToAdd: null))
			.AddInterceptors(sp.GetRequiredService<SecondLevelCacheInterceptor>());
	},
	(IServiceProvider sp) => sp.GetRequiredService<RedisConnectionManager>().Database);

const string ef2ndCacheName = "ef2nd_cache";
string redisPrefix = "tgs-gamebets-efcache:";
builder.Services
	.AddEFSecondLevelCache(options =>
		options
			.UseEasyCachingCoreProvider(
				providerName: ef2ndCacheName,
				isHybridCache: false)
			.UseCacheKeyPrefix(redisPrefix)
			.UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1))
			.CacheQueriesContainingTypes(
				expirationMode: CacheExpirationMode.Absolute,
				timeout: TimeSpan.FromMinutes(10),
				tableTypeComparison: TableTypeComparison.ContainsOnly,
				entityTypes: [typeof(Tgs.GameBets.Entities.GameBetRunning)])
	)
	// 不能使用 AddEasyCaching(Action<IServiceProvider, EasyCachingOptions>) 版 delegate 來取得 Configuration
	// 有無法順利註冊服務的 bug. 需要使用 AddEasyCaching(Action<EasyCachingOptions>) 直接從 web app builder 拿
	// ref: https://github.com/dotnetcore/EasyCaching/issues/273
	.AddEasyCaching(options =>
	{
		var connectionString = builder.Configuration.GetConnectionString("Redis")!;
		var configurationOptions = ConfigurationOptions.Parse(connectionString);
		const string ezCacheSeriailzerName = "ez_cache_serializer";
		options
			.UseRedis(
				configure: config =>
				{
					config.DBConfig.ConfigurationOptions = configurationOptions;
					config.SerializerName = ezCacheSeriailzerName;
					// config.EnableLogging = true; // 可以從 log 中確認 cache missed 或 cache hit
				},
				name: ef2ndCacheName)
			.WithMessagePack(ezCacheSeriailzerName);
	});

builder.Services
	.AddOpenTelemetry()
	.WithTracing(builder => builder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddSource("MediatR")
		.AddHttpClientInstrumentation()
		.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService($"{Environment.OSVersion}"))
		.AddAspNetCoreInstrumentation(options => options.Filter =
		httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/healthz")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !httpContext.Request.Path.StartsWithSegments("/hangfire")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddNpgsql()
		.AddHttpClientInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddOtlpExporter())
	.WithMetrics(meterBuiler => meterBuiler
	.SetResourceBuilder(ResourceBuilder.CreateDefault()
		.AddEnvironmentVariableDetector())
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter());

builder.Services.AddHealthChecks();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.MapControllers();
_ = app.MapSwagger();

app.UseHealthChecks("/healthz");

app.Run();
