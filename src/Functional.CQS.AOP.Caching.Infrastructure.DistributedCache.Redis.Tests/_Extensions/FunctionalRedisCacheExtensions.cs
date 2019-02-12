using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests._Models;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests._Extensions
{
	internal static class FunctionalRedisCacheExtensions
	{
		public static void VerifyNoNewItemsHaveBeenAdded(this FunctionalRedisCache sut, IEnumerable<CacheItem> originalCollectionOfCacheItems)
		{
			sut.ItemCount.Should().Be(originalCollectionOfCacheItems.Count(), "no item was added");
			sut.KeyToGroupKeyItemCount.Should().Be(originalCollectionOfCacheItems.Count(), "no item was added");
			sut.GroupKeySetItemCount.Should().Be(originalCollectionOfCacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count(), "no item was added");
		}

		public static void VerifyOneNewItemHasBeenAdded(this FunctionalRedisCache sut, IEnumerable<CacheItem> originalCollectionOfCacheItems, Option<string> groupKey)
		{
			// ItemCount and CancellationTokenCountByKey should both increment by one, but CancellationTokenCountByGroupKey should only be incremented by one if groupKey is new to cache; otherwise, it should be unchanged
			sut.ItemCount.Should().Be(originalCollectionOfCacheItems.Count() + 1, "an item was added");
			sut.KeyToGroupKeyItemCount.Should().Be(originalCollectionOfCacheItems.Count() + groupKey.Match(_ => 1, () => 0), "an item was added");
			sut.GroupKeySetItemCount.Should().Be(
				originalCollectionOfCacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Select(x => x.Key).Union(groupKey.ToEnumerable()).Distinct().Count(),
				groupKey.Match(gk => originalCollectionOfCacheItems.Any(x => x.GroupKey.ValueOrDefault() == gk) ? "the group key is already in the cache" : "the group key was not already in the cache", () => "no group key was given"));
		}
	}

	internal static class OptionExtensions
	{
		public static IEnumerable<T> ToEnumerable<T>(this Option<T> source) => source.Match(value => Enumerable.Repeat(value, 1), Enumerable.Empty<T>);
	}
}