using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;
using FluentAssertions;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests._Extensions;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests._Models;
using Functional.Primitives.FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests
{
	// ReSharper disable once InconsistentNaming
	public class FunctionalRedisCacheTests
	{
		// -- IMPORTANT --
		// These tests require a Redis instance to be running locally on your machine; otherwise, they will all fail.
		// The easiest way to get Redis running on your machine is to use Docker.
		//
		// 1. If you do not have Docker already installed, do so
		// 2. Open a Powershell window as administrator
		// 3. Execute the following command: "docker run --name FunctionalRedis -p 6379:6379 -d redis"
		//    - we name the container FunctionalRedis for easy identification
		//    - this command spins up a Redis instance that listens on default port 6379
		// 4. Run the tests
		//
		// Additional documentation about redis in Docker can be found at https://hub.docker.com/_/redis/
		// In PowerShell, you can execute the following command to link redis-cli to the running Redis instance: "docker run -it --link FunctionalRedis:redis --rm redis redis-cli -h redis -p 6379"
		//
		// These tests will successfully run in pull requests using Travis CI because the build script sets up a Redis instance prior to executing the build and running all tests

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldCountNumberOfItemsProperly(FunctionalRedisCache sut)
		{
			// make sure cache is in correct initial state
			sut.ItemCount.Should().Be(_cacheItems.Count());
			sut.KeyToGroupKeyItemCount.Should().Be(_cacheItems.Count());
			sut.GroupKeySetItemCount.Should().Be(_cacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count());
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldAddItemBelongingToNoGroupToCache(FunctionalRedisCache sut) => ShouldAddItemToCache_Impl(sut, Option.None<string>());

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldAddItemBelongingToNewGroupToCache(FunctionalRedisCache sut) => ShouldAddItemToCache_Impl(sut, _testGroupNotInOriginalCache);

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldAddItemBelongingToExistingGroupToCache(FunctionalRedisCache sut) => ShouldAddItemToCache_Impl(sut, _test3Group);

		private void ShouldAddItemToCache_Impl(FunctionalRedisCache sut, Option<string> groupKey)
		{
			sut.ItemCount.Should().Be(_cacheItems.Count()); // make sure cache is in correct initial state
			sut.Add(KEY_NOT_IN_ORIGINAL_CACHE, groupKey, new object(), TimeSpan.FromMinutes(5));
			sut.VerifyOneNewItemHasBeenAdded(_cacheItems, groupKey);
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldAddItemToCacheWhenRetrievingItemAndItIsNotAlreadyStoredInTheCache_NonGenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public void ShouldRetrieveExistingItemFromCacheWhenRetrievingItemAndItIsAlreadyStoredInTheCache_NonGenericOverload(FunctionalRedisCache sut)
		{
			sut.Contains(KEY3).Should().BeTrue(); // make sure cache is in correct initial state
			var result = sut.Get(KEY3, Option.None<string>(), _cacheItemLookup[KEY3].GetType(), () => (SampleStructData)_cacheItemLookup[KEY3], _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY3).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be((SampleStructData)_cacheItemLookup[KEY3]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public async Task ShouldOnlyCallDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_NonGenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public void ShouldAddItemToCacheWhenRetrievingItemAndItIsNotAlreadyStoredInTheCache_GenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public void ShouldRetrieveExistingItemFromCacheWhenRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload(FunctionalRedisCache sut)
		{
			sut.Contains(KEY1).Should().BeTrue(); // make sure cache is in correct initial state
			var result = sut.Get(KEY1, Option.None<string>(), () => (string)_cacheItemLookup[KEY1], _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY1).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be((string)_cacheItemLookup[KEY1]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldRetrieveExistingItemFromCacheWhenRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload_ObjectStoredInCacheIsEnumerable(FunctionalRedisCache sut)
		{
			sut.Contains(KEY2A).Should().BeTrue(); // make sure cache is in correct initial state
			var result = sut.Get(KEY2A, Option.None<string>(), () => (IEnumerable<int>)_cacheItemLookup[KEY2A], _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY2A).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().BeEquivalentTo((IEnumerable<int>)_cacheItemLookup[KEY2A]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldRetrieveExistingItemFromCacheWhenRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload_ObjectStoredInCacheHasInternalEnumerable(FunctionalRedisCache sut)
		{
			sut.Contains(KEY2B).Should().BeTrue(); // make sure cache is in correct initial state
			var result = sut.Get(KEY2B, Option.None<string>(), () => (SampleReferenceData)_cacheItemLookup[KEY2B], _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY2B).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be((SampleReferenceData)_cacheItemLookup[KEY2B]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldNotStoreOptionInCacheIfConfiguredToNotStoreNone(FunctionalRedisCache sut)
		{
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Option<int>), () => Option.None<int>(), option => ((Option<int>)option).HasValue(), TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Option<int>)value).Should().NotHaveValue());
			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeFalse("configured to not store Option.None in the cache");
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldReturnOptionSomeItemFromCache(FunctionalRedisCache sut)
		{
			const int VALUE = 1337;
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Option<int>), () => Option.Some(VALUE), option => ((Option<int>)option).HasValue(), TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Option<int>)value).Should().HaveExpectedValue(VALUE));
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Option<int>), () => throw new NotImplementedException(), option => ((Option<int>)option).HasValue(), TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Option<int>)value).Should().HaveExpectedValue(VALUE));
			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue("configured to store Option.Some in the cache");
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldReturnOptionNoneItemFromCache(FunctionalRedisCache sut)
		{
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Option<int>), () => Option.None<int>(), option => true, TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Option<int>)value).Should().NotHaveValue());
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Option<int>), () => throw new NotImplementedException(), option => true, TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Option<int>)value).Should().NotHaveValue());
			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue("configured to store Option.None in the cache");
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldNotStoreResultInCacheIfConfiguredToNotStoreFaultedResult(FunctionalRedisCache sut)
		{
			const string FAIL_VALUE = "all your base are belong to us";
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Result<int, string>), () => Result.Failure<int, string>(FAIL_VALUE), result => ((Result<int, string>)result).IsSuccess(), TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Result<int, string>)value).Should().BeFaultedWithExpectedValue(FAIL_VALUE));
			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeFalse("configured to not store Result.Fail in the cache");
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldReturnSuccessfulResultItemFromCache(FunctionalRedisCache sut)
		{
			const int SUCCESS_VALUE = 1337;
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Result<int, string>), () => Result.Success<int, string>(SUCCESS_VALUE), result => ((Result<int, string>)result).IsSuccess(), TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Result<int, string>)value).Should().BeSuccessfulWithExpectedValue(SUCCESS_VALUE));
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Result<int, string>), () => throw new NotImplementedException(), result => ((Result<int, string>)result).IsSuccess(), TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Result<int, string>)value).Should().BeSuccessfulWithExpectedValue(SUCCESS_VALUE));
			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue("configured to store Result.Success in the cache");
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldReturnFaultedResultItemFromCache(FunctionalRedisCache sut)
		{
			const string FAIL_VALUE = "all your base are belong to us";
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Result<int, string>), () => Result.Failure<int, string>(FAIL_VALUE), result => true, TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Result<int, string>)value).Should().BeFaultedWithExpectedValue(FAIL_VALUE));
			sut.Get(KEY_NOT_IN_ORIGINAL_CACHE, Option.None<string>(), typeof(Result<int, string>), () => throw new NotImplementedException(), result => true, TimeSpan.FromMinutes(5)).Should().BeSuccessful(value => ((Result<int, string>)value).Should().BeFaultedWithExpectedValue(FAIL_VALUE));
			sut.Contains(KEY_NOT_IN_ORIGINAL_CACHE).Should().BeTrue("configured to store Result.Fail in the cache");
		}

		[Theory]
		[DistributedCacheArrangement]
		public async Task ShouldOnlyCallDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_GenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public async Task ShouldAddItemToCacheWhenAsynchronouslyRetrievingItemAndItIsNotAlreadyStoredInTheCache_NonGenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public async Task ShouldRetrieveExistingItemFromCacheWhenAsynchronouslyRetrievingItemAndItIsAlreadyStoredInTheCache_NonGenericOverload(FunctionalRedisCache sut)
		{
			sut.Contains(KEY3).Should().BeTrue();
			var result = await sut.GetAsync(KEY3, Option.None<string>(), _cacheItemLookup[KEY3].GetType(), () => Task.FromResult<object>((SampleStructData)_cacheItemLookup[KEY3]), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY3).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be((SampleStructData)_cacheItemLookup[KEY3]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public async Task ShouldOnlyCallAsyncDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_NonGenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public async Task ShouldAddItemToCacheWhenAsynchronouslyRetrievingItemAndItIsNotAlreadyStoredInTheCache_GenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public async Task ShouldRetrieveExistingItemFromCacheWhenAsynchronouslyRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload(FunctionalRedisCache sut)
		{
			var groupKey = Option.None<string>();

			sut.Contains(KEY1).Should().BeTrue(); // make sure cache is in correct initial state
			var result = await sut.GetAsync(KEY1, groupKey, () => Task.FromResult((string)_cacheItemLookup[KEY1]), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY1).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be((string)_cacheItemLookup[KEY1]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public async Task ShouldRetrieveExistingItemFromCacheWhenAsynchronouslyRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload_ObjectStoredInCacheIsEnumerable(FunctionalRedisCache sut)
		{
			var groupKey = Option.None<string>();

			sut.Contains(KEY2A).Should().BeTrue(); // make sure cache is in correct initial state
			var result = await sut.GetAsync(KEY2A, groupKey, () => Task.FromResult((IEnumerable<int>)_cacheItemLookup[KEY2A]), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY2A).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().BeEquivalentTo((IEnumerable<int>)_cacheItemLookup[KEY2A]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public async Task ShouldRetrieveExistingItemFromCacheWhenAsynchronouslyRetrievingItemAndItIsAlreadyStoredInTheCache_GenericOverload_ObjectStoredInCacheHasInternalEnumerable(FunctionalRedisCache sut)
		{
			var groupKey = Option.None<string>();

			sut.Contains(KEY2B).Should().BeTrue(); // make sure cache is in correct initial state
			var result = await sut.GetAsync(KEY2B, groupKey, () => Task.FromResult((SampleReferenceData)_cacheItemLookup[KEY2B]), _ => true, TimeSpan.FromMinutes(5));

			sut.Contains(KEY2B).Should().BeTrue();
			result.Should().BeSuccessful(value => value.Should().Be((SampleReferenceData)_cacheItemLookup[KEY2B]));
			sut.VerifyNoNewItemsHaveBeenAdded(_cacheItems);
		}

		[Theory]
		[DistributedCacheArrangement]
		public async Task ShouldOnlyCallAsyncDataRetrievalMethodOnceWhenMultipleThreadsAccessCacheAtTheSameTime_GenericOverload(FunctionalRedisCache sut)
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
		[DistributedCacheArrangement]
		public void ShouldRemoveItemWithCorrespondingKey(FunctionalRedisCache sut)
		{
			sut.Contains(KEY1).Should().BeTrue(); // make sure cache is in correct initial state
			sut.Remove(KEY1);

			Thread.Sleep(1000); // cache is updated on a background thread, so sleep for a bit
			sut.Contains(KEY1).Should().BeFalse();
			sut.ItemCount.Should().Be(_cacheItems.Count(x => x.Key != KEY1), "the item should have been removed");
			sut.KeyToGroupKeyItemCount.Should().Be(_cacheItems.Count(x => x.Key != KEY1), "group key associations of removed items should have been removed as well");
			sut.GroupKeySetItemCount.Should().Be(_cacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count(x => x.Key != TEST_GROUP1), "empty groups should have been removed as well");
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldRemoveAllItemsWithGroupKeyFromTheCache(FunctionalRedisCache sut)
		{
			// make sure cache is in correct initial state
			sut.ItemCount.Should().Be(_cacheItems.Count());
			sut.KeyToGroupKeyItemCount.Should().Be(_cacheItems.Count());
			sut.GroupKeySetItemCount.Should().Be(_cacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count());
			sut.RemoveGroup(TEST_GROUP2);

			Thread.Sleep(1000); // cache is updated on a background thread, so sleep for a bit
			sut.ItemCount.Should().Be(_cacheItems.Count(x => x.GroupKey.ValueOrDefault() != TEST_GROUP2), "items should have been removed");
			sut.KeyToGroupKeyItemCount.Should().Be(_cacheItems.Count(x => x.GroupKey.ValueOrDefault() != TEST_GROUP2), "cancellation tokens associated with removed items should have been removed as well");
			sut.GroupKeySetItemCount.Should().Be(_cacheItems.GroupBy(x => x.GroupKey.ValueOrDefault()).Count(x => x.Key != TEST_GROUP2), "cancellation tokens associated with removed item groups should have been removed as well");
		}

		[Theory]
		[DistributedCacheArrangement]
		public void ShouldRemoveAllItemsFromTheCache(FunctionalRedisCache sut)
		{
			sut.ItemCount.Should().Be(_cacheItems.Count()); // make sure cache is in correct initial state
			sut.Clear();

			Thread.Sleep(1000); // cache is updated on a background thread, so sleep for a bit
			sut.ItemCount.Should().Be(0);
			sut.KeyToGroupKeyItemCount.Should().Be(0);
			sut.GroupKeySetItemCount.Should().Be(0);
		}

		#region Arrangements

		private class DistributedCacheArrangement : AutoDataAttribute
		{
			public DistributedCacheArrangement()
				: base(() => new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new DistributedCacheCustomization()))
			{
			}
		}

		#endregion

		#region Customizations

		private class DistributedCacheCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				// localhost = 127.0.0.1:6379 (per Redis convention at https://stackexchange.github.io/StackExchange.Redis/Basics)
				var cache = new FunctionalRedisCache(FunctionalRedisCacheConfiguration.ForLocalHostPort6379());
				cache.Clear();
				foreach (var item in _cacheItems)
					cache.Add(item.Key, item.GroupKey, _cacheItemLookup[item.Key], TimeSpan.FromMinutes(5));

				fixture.Inject(cache);
			}
		}

		#endregion

		#region Models

		private class SampleReferenceData
		{
			public SampleReferenceData(int id, IEnumerable<int> data)
			{
				ID = id;
				Data = data;
			}

			public int ID { get; }
			public IEnumerable<int> Data { get; }

			// needed for FluentAssertions to make correct equality comparison
			public override bool Equals(object b)
			{
				if (!(b is SampleReferenceData @object))
					return false;

				var items = Data.ToArray();
				var otherItems = @object.Data.ToArray();
				if (items.Length != otherItems.Length)
					return false;

				for (int n = 0; n < items.Length; ++n)
				{
					if (items[n] != otherItems[n])
						return false;
				}

				return true;
			}

			// this doesn't actually matter for the tests
			public override int GetHashCode() => 1337;
		}

		private struct SampleStructData
		{
			public SampleStructData(int[] data)
			{
				Data = data ?? throw new ArgumentNullException(nameof(data));
			}

			public int[] Data { get; }

			public override bool Equals(object b)
			{
				if (!(b is SampleStructData @object))
					return false;

				var items = Data.ToArray();
				var otherItems = @object.Data.ToArray();
				if (items.Length != otherItems.Length)
					return false;

				for (int n = 0; n < items.Length; ++n)
				{
					if (items[n] != otherItems[n])
						return false;
				}

				return true;
			}

			public override int GetHashCode() => base.GetHashCode();
		}

		#endregion

		#region Data

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
			{ KEY1, "value1" },
			{ KEY2A, Enumerable.Range(1, 3) },
			{ KEY2B, new SampleReferenceData(666, Enumerable.Range(1, 5)) },
			{ KEY2C, "value2c" },
			{ KEY2D, "value2d" },
			{ KEY3, new SampleStructData(new[] { 1, 2, 3} )}
		};

		#endregion
	}

	internal static class ObjectExtensions
	{
		public static Task<T> AsTask<T>(this T obj) => Task.FromResult(obj);
	}
}
