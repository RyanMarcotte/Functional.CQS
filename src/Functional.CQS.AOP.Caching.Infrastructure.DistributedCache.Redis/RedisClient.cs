using StackExchange.Redis;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	internal class RedisClient
	{
		public RedisClient(IServer server, IDatabase database)
		{
			Server = server;
			Database = database;
		}

		public IServer Server { get; }
		public IDatabase Database { get; }
	}
}