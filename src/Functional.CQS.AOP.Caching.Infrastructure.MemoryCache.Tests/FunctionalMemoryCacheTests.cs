using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;
using FluentAssertions;
using Functional.Primitives.FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.Caching.Infrastructure.MemoryCache.Tests
{
	public class FunctionalMemoryCacheTests
	{
		[Theory]
		[MemoryCacheArrangement]
		public void ShouldAddItemBelongingToNoGroupToCache(FunctionalMemoryCache sut) => ShouldAddItemToCache_Impl(sut, Option.None<string>());

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldAddItemBelongingToNewGroupToCache(FunctionalMemoryCache sut) => ShouldAddItemToCache_Impl(sut, _testGroupNotInOriginalCache);

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldAddItemBelongingToExistingGroupToCache(FunctionalMemoryCache sut) => ShouldAddItemToCache_Impl(sut, _test3Group);

		private void ShouldAddItemToCache_Impl(FunctionalMemoryCache sut, Option<string> groupKey)
		{
			sut.ItemCount.Should().Be(_cacheItems.Count()); // make sure cache is in correct initial state
			sut.Add(KEY_NOT_IN_ORIGINAL_CACHE, groupKey, new object(), TimeSpan.FromMinutes(5));
			sut.VerifyOneNewItemHasBeenAdded(_cacheItems, groupKey);
		}

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldAddItemToCacheWhenRetrievingItemAndItIsNotAlreadyStoredInTheCache_NonGenericOverload(FunctionalMemoryCache sut)
		{
			const int KEY_TO_STORE = 5;

			var groupKey = Option.None<string>();
			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeFalse(); // make sure cache is in correct initial state
			var result = sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, groupKey, typeof(int), () => KEY_TO_STORE, _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(KEY_TO_STORE));
			sut.VerifyOneNewItemHasBeenAdded(_cacheItems, groupKey);
		}

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldRetrieveExistingItemFromCacheWhenRetrievingItemAndItIsAlreadyStoredInTheCache_NonGenericOverload(FunctionalMemoryCache sut)
		{
			sut.Contains(KEY1).Should().BeTrue(); // make sure cache is in correct initial state
			var result = sut.Get(KEY1, Option.None<string>(), typeof(object), () => _cacheItemLookup[KEY1], _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY1).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(_cacheItemLookup[KEY1]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldOnlyCallDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_NonGenericOverload(FunctionalMemoryCache sut)
		{
			int numberOfCalls = 0;
			await Parallel.ForEach(Enumerable.Range(0, 25), n => sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(int), () =>
			{
				Interlocked.Increment(ref numberOfCalls);
				return n;
			}, _ => true, TimeSpan.FromMinutes(5))).AsTask();

			numberOfCalls.Should().Be(1);
		}

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldAddItemToCacheWhenRetrievingItemAndItIsNotAlreadyStoredInTheCache_GenericOverload(FunctionalMemoryCache sut)
		{
			var groupKey = Option.None<string>();
			var itemToStore = new object();

			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeFalse(); // make sure cache is in correct initial state
			var result = sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, groupKey, () => itemToStore, _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(itemToStore));
			sut.VerifyOneNewItemHasBeenAdded(_cacheItems, groupKey);
		}

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldRetrieveExistingItemFromCacheWhenRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload(FunctionalMemoryCache sut)
		{
			sut.Contains(KEY1).Should().BeTrue(); // make sure cache is in correct initial state
			var result = sut.Get(KEY1, Option.None<string>(), () => _cacheItemLookup[KEY1], _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY1).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(_cacheItemLookup[KEY1]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldOnlyCallDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_GenericOverload(FunctionalMemoryCache sut)
		{
			int numberOfCalls = 0;
			await Parallel.ForEach(Enumerable.Range(0, 25), n => sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), () =>
			{
				Interlocked.Increment(ref numberOfCalls);
				return n.ToString();
			}, _ => true, TimeSpan.FromMinutes(5))).AsTask();

			numberOfCalls.Should().Be(1);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldAddItemToCacheWhenAsynchronouslyRetrievingItemAndItIsNotAlreadyStoredInTheCache_NonGenericOverload(FunctionalMemoryCache sut)
		{
			const int KEY_TO_STORE = 5;
			var groupKey = Option.None<string>();

			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeFalse();
			var result = await sut.GetAsync(KEY_NOT_IN_ORIGINAL_CACHE, groupKey, typeof(int), () => Task.FromResult((object)KEY_TO_STORE), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(KEY_TO_STORE));
			sut.VerifyOneNewItemHasBeenAdded(_cacheItems, groupKey);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldRetrieveExistingItemFromCacheWhenAsynchronouslyRetrievingItemAndItIsAlreadyStoredInTheCache_NonGenericOverload(FunctionalMemoryCache sut)
		{
			sut.Contains(KEY1).Should().BeTrue();
			var result = await sut.GetAsync(KEY1, Option.None<string>(), typeof(object), () => Task.FromResult(_cacheItemLookup[KEY1]), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY1).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(_cacheItemLookup[KEY1]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldOnlyCallAsyncDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_NonGenericOverload(FunctionalMemoryCache sut)
		{
			int numberOfCalls = 0;
			var tasks = Enumerable.Range(0, 25).Select(x => Task.Run(() => sut.GetAsync(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(int), async () =>
			{
				Interlocked.Increment(ref numberOfCalls);
				return await Task.FromResult(x);
			}, _ => true, TimeSpan.FromMinutes(5)))).Cast<Task>().ToArray();

			await Task.WhenAll(tasks);
			numberOfCalls.Should().Be(1);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldAddItemToCacheWhenAsynchronouslyRetrievingItemAndItIsNotAlreadyStoredInTheCache_GenericOverload(FunctionalMemoryCache sut)
		{
			var groupKey = Option.None<string>();
			var itemToStore = new object();

			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeFalse(); // make sure cache is in correct initial state
			var result = await sut.GetAsync(KEY_NOT_IN_ORIGINAL_CACHE, groupKey, () => Task.FromResult(itemToStore), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(itemToStore));
			sut.VerifyOneNewItemHasBeenAdded(_cacheItems, groupKey);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldRetrieveExistingItemFromCacheWhenAsynchronouslyRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload(FunctionalMemoryCache sut)
		{
			var groupKey = Option.None<string>();

			sut.Contains(KEY1).Should().BeTrue(); // make sure cache is in correct initial state
			var result = await sut.GetAsync(KEY1, groupKey, () => Task.FromResult(_cacheItemLookup[KEY1]), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY1).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be(_cacheItemLookup[KEY1]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[MemoryCacheArrangement]
		public async Task ShouldOnlyCallAsyncDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_GenericOverload(FunctionalMemoryCache sut)
		{
			int numberOfCalls = 0;
			var tasks = Enumerable.Range(0, 25).Select(x => Task.Run(() => sut.GetAsync(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), async () =>
			{
				Interlocked.Increment(ref numberOfCalls);
				return await Task.FromResult(new object());
			}, _ => true, TimeSpan.FromMinutes(5)))).Cast<Task>().ToArray();

			await Task.WhenAll(tasks);
			numberOfCalls.Should().Be(1);
		}

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldRemoveItemWithCorrespondingKey(FunctionalMemoryCache sut)
		{
			sut.Contains(KEY1).Should().BeTrue(); // make sure cache is in correct initial state
			sut.Remove(KEY1).Should().BeSuccessful();

			Thread.Sleep(1000); // cache is updated on a background thread, so sleep for a bit
			sut.Contains(KEY1).Should().BeFalse();
			sut.ItemCount.Should().Be(_cacheItems.Count(x => x.Key != KEY1), "the item should have been removed");
			sut.CancellationTokenCountByKey.Should().Be(_cacheItems.Count(x => x.Key != KEY1), "cancellation tokens associated with removed items should have been removed as well");
			sut.CancellationTokenCountByGroupKey.Should().Be(_cacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count(x => x.Key != TEST_GROUP1), "cancellation tokens associated with removed item groups should have been removed as well");
		}

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldRemoveAllItemsWithGroupKeyFromTheCache(FunctionalMemoryCache sut)
		{
			// make sure cache is in correct initial state
			sut.ItemCount.Should().Be(_cacheItems.Count());
			sut.CancellationTokenCountByKey.Should().Be(_cacheItems.Count());
			sut.CancellationTokenCountByGroupKey.Should().Be(_cacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count());
			sut.RemoveGroup(TEST_GROUP2).Should().BeSuccessful();

			Thread.Sleep(1000); // cache is updated on a background thread, so sleep for a bit
			sut.ItemCount.Should().Be(_cacheItems.Count(x => x.GroupKey.ValueOrDefault() != TEST_GROUP2), "items should have been removed");
			sut.CancellationTokenCountByKey.Should().Be(_cacheItems.Count(x => x.GroupKey.ValueOrDefault() != TEST_GROUP2), "cancellation tokens associated with removed items should have been removed as well");
			sut.CancellationTokenCountByGroupKey.Should().Be(_cacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count(x => x.Key != TEST_GROUP2), "cancellation tokens associated with removed item groups should have been removed as well");
		}

		[Theory]
		[MemoryCacheArrangement]
		public void ShouldRemoveAllItemsFromTheCache(FunctionalMemoryCache sut)
		{
			sut.ItemCount.Should().Be(_cacheItems.Count()); // make sure cache is in correct initial state
			sut.Clear().Should().BeSuccessful();

			Thread.Sleep(1000); // cache is updated on a background thread, so sleep for a bit
			sut.ItemCount.Should().Be(0);
		}

		#region Arrangements

		private class MemoryCacheArrangement : AutoDataAttribute
		{
			public MemoryCacheArrangement()
				: base(() => new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new MemoryCacheCustomization()))
			{
			}
		}

		#endregion

		#region Customizations

		private class MemoryCacheCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Register(() =>
				{
					var cache = new FunctionalMemoryCache();
					foreach (var item in _cacheItems)
						cache.Add(item.Key, item.GroupKey, _cacheItemLookup[item.Key], TimeSpan.FromMinutes(5));
					return cache;
				});
			}
		}

		#endregion

		#region Models

		private const string KEY1 = "1";
		private const string KEY2A = "2a";
		private const string KEY2B = "2b";
		private const string KEY2C = "2c";
		private const string KEY2D = "2d";
		private const string KEY3 = "3";
		private const string KEY_NOT_IN_ORIGINAL_CACHE = "4";

		private const string TEST_GROUP1 = "test1";
		private const string TEST_GROUP2 = "test2";
		private const string TEST_GROUP3 = "test3";
		private const string ITEM_GROUP_NOT_IN_ORIGINAL_CACHE = "test4";

		private static readonly Option<string> _test1Group = Option.Some(TEST_GROUP1);
		private static readonly Option<string> _test2Group = Option.Some(TEST_GROUP2);
		private static readonly Option<string> _test3Group = Option.Some(TEST_GROUP3);
		private static readonly Option<string> _testGroupNotInOriginalCache = Option.Some(ITEM_GROUP_NOT_IN_ORIGINAL_CACHE);

		private static readonly IEnumerable<CacheItem> _cacheItems = new[]
		{
			new CacheItem(KEY1, _test1Group),
			new CacheItem(KEY2A, _test2Group),
			new CacheItem(KEY2B, _test2Group),
			new CacheItem(KEY2C, _test2Group),
			new CacheItem(KEY2D, _test2Group),
			new CacheItem(KEY3, _test3Group)
		};

		private static readonly IReadOnlyDictionary<string, object> _cacheItemLookup = new Dictionary<string, object>
		{
			{ KEY1, new object() },
			{ KEY2A, new object() },
			{ KEY2B, new object() },
			{ KEY2C, new object() },
			{ KEY2D, new object() },
			{ KEY3, new object() }
		};

		#endregion
	}

	internal static class ObjectExtensions
	{
		public static Task<T> AsTask<T>(this T obj) => Task.FromResult(obj);
	}
}
