using StackExchange.Redis;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	internal static class ConnectionMultiplexerExtensions
	{
		public static IServer GetServer(this ConnectionMultiplexer source, FunctionalRedisCacheConfiguration configuration)
			=> source.GetServer(configuration.HostURL, configuration.PortNumber);

		public static RedisClient GetRedisClient(this ConnectionMultiplexer source, FunctionalRedisCacheConfiguration configuration)
			=> new RedisClient(source.GetServer(configuration), source.GetDatabase());
	}
}