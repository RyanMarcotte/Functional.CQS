namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests._Models
{
	internal class CacheItem
	{
		public CacheItem(string key, Option<string> groupKey)
		{
			Key = key;
			GroupKey = groupKey;
		}

		public string Key { get; }
		public Option<string> GroupKey { get; }
	}
}