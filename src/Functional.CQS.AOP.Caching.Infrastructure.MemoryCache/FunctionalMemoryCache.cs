using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Functional.CQS.AOP.Caching.Infrastructure.MemoryCache
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Used to cache data in-memory.
	/// </summary>
	public class FunctionalMemoryCache : IFunctionalCache
	{
		private readonly Microsoft.Extensions.Caching.Memory.MemoryCache _cache;
		private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreSlimLookup = new ConcurrentDictionary<string, SemaphoreSlim>();
		private readonly ConcurrentDictionary<string, string> _keyToGroupKeyAssociationLookup = new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _groupKeyToKeyAssociationLookup = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>(); // no ConcurrentHashSet<T>, so use ConcurrentDictionary<string, byte>; https://stackoverflow.com/questions/18922985/concurrent-hashsett-in-net-framework
		private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokenSourceLookupByKey = new ConcurrentDictionary<string, CancellationTokenSource>();
		private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokenSourceLookupByGroupKey = new ConcurrentDictionary<string, CancellationTokenSource>();

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalMemoryCache"/> class.
		/// </summary>
		public FunctionalMemoryCache(MemoryCacheOptions options)
		{
			_cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(options);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalMemoryCache"/> class.
		/// </summary>
		public FunctionalMemoryCache()
			: this(new MemoryCacheOptions())
		{

		}

		/// <summary>
		/// Adds an item to the cache.
		/// </summary>
		/// <typeparam name="T">The type of item to add.</typeparam>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.</param>
		/// <param name="item">The item to add.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		/// <returns></returns>
		public Result<Unit, Exception> Add<T>(string key, Option<string> groupKey, T item, TimeSpan timeToLive)
		{
			return Result.Try(() =>
			{
				groupKey.Apply(gk =>
				{
					_keyToGroupKeyAssociationLookup.GetOrAdd(key, _ => gk);
					_groupKeyToKeyAssociationLookup.GetOrAdd(gk, _ => new ConcurrentDictionary<string, byte>()).TryAdd(key, 0);
				});

				var cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetPriority(CacheItemPriority.Normal)
					.SetAbsoluteExpiration(DateTimeOffset.UtcNow.Add(timeToLive))
					.AddExpirationToken(new CancellationChangeToken(_cancellationTokenSourceLookupByKey.GetOrAdd(key, _ => new CancellationTokenSource()).Token))
					.RegisterPostEvictionCallback(DoPostEvictionCleanup);

				// if a group key is specified, then create a cancellation token that can be used to forcefully expire that item
				groupKey.Select(gk => _cancellationTokenSourceLookupByGroupKey.GetOrAdd(gk, _ => new CancellationTokenSource())).Apply(
					cts => _cache.Set(key, item, cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cts.Token))),
					() => _cache.Set(key, item, cacheEntryOptions));

				return Unit.Value;
			});
		}

		/// <summary>
		/// Indicates if the cache contains the specified key.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <returns></returns>
		public bool Contains(string key)
		{
			return _cache.TryGetValue(key, out var _);
		}

		/// <summary>
		/// Retrieves an item from the cache.  If the item does not exist, it will be added.
		/// </summary>
		/// <typeparam name="T">The type of item to retrieve.</typeparam>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.</param>
		/// <param name="dataRetriever">If the item does not exist in the cache, this function is called to generate the item and then add it to the cache.</param>
		/// <param name="shouldCacheData">If the item does not exist in the cache, this function is called after the item is retrieved to determine if it should be added to the cache.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		/// <returns></returns>
		public Result<T, Exception> Get<T>(string key, Option<string> groupKey, Func<T> dataRetriever, Func<T, bool> shouldCacheData, TimeSpan timeToLive) where T : class
		{
			return TryGetItem<T>(key).Match(
				item => Result.Success<T, Exception>(item),
				() => ExecuteDataRetrieval(key, groupKey, dataRetriever, shouldCacheData, x => x, timeToLive));
		}

		/// <summary>
		/// Retrieves an item from the cache.  If the item does not exist, it will be added.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.</param>
		/// <param name="type">The type of item to retrieve.</param>
		/// <param name="dataRetriever">If the item does not exist in the cache, this function is called to generate the item and then add it to the cache.</param>
		/// <param name="shouldCacheData">If the item does not exist in the cache, this function is called after the item is retrieved to determine if it should be added to the cache.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		/// <returns></returns>
		public Result<object, Exception> Get(string key, Option<string> groupKey, Type type, Func<object> dataRetriever, Func<object, bool> shouldCacheData, TimeSpan timeToLive)
		{
			return TryGetItem(key, type).Match(
				item => Result.Success<object, Exception>(item),
				() => ExecuteDataRetrieval(key, groupKey, dataRetriever, shouldCacheData, x => NullIfNotSpecifiedType(x, type), timeToLive));
		}

		/// <summary>
		/// Retrieves an item from the cache asynchronously.  If the item does not exist, it will be added.
		/// </summary>
		/// <typeparam name="T">The type of item to retrieve.</typeparam>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.  If null, the cached item will not belong to a group.</param>
		/// <param name="dataRetriever">If the item does not exist in the cache, this function is called to generate the item and then add it to the cache.</param>
		/// <param name="shouldCacheData">If the item does not exist in the cache, this function is called after the item is retrieved to determine if it should be added to the cache.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		/// <returns></returns>
		public async Task<Result<T, Exception>> GetAsync<T>(string key, Option<string> groupKey, Func<Task<T>> dataRetriever, Func<T, bool> shouldCacheData, TimeSpan timeToLive) where T : class
		{
			return await TryGetItem<T>(key).Match(
				async item => await Task.FromResult(Result.Success<T, Exception>(item)),
				async () => await ExecuteAsyncDataRetrieval(key, groupKey, dataRetriever, shouldCacheData, x => x, timeToLive));
		}

		/// <summary>
		/// Retrieves an item from the cache asynchronously.  If the item does not exist, it will be added.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.  If null, the cached item will not belong to a group.</param>
		/// <param name="type">The type of item to retrieve.</param>
		/// <param name="dataRetriever">If the item does not exist in the cache, this function is called to generate the item and then add it to the cache.</param>
		/// <param name="shouldCacheData">If the item does not exist in the cache, this function is called after the item is retrieved to determine if it should be added to the cache.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		/// <returns></returns>
		public async Task<Result<object, Exception>> GetAsync(string key, Option<string> groupKey, Type type, Func<Task<object>> dataRetriever, Func<object, bool> shouldCacheData, TimeSpan timeToLive)
		{
			return await TryGetItem(key, type).Match(
				async item => await Task.FromResult(Result.Success<object, Exception>(item)),
				async () => await ExecuteAsyncDataRetrieval(key, groupKey, dataRetriever, shouldCacheData, x => NullIfNotSpecifiedType(x, type), timeToLive));
		}

		/// <summary>
		/// Removes an item from the cache.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		public Result<Unit, Exception> Remove(string key)
		{
			return Result.Try(() =>
			{
				_cancellationTokenSourceLookupByKey.TryRemove(key).Apply(cts => cts.Cancel());
				_keyToGroupKeyAssociationLookup.TryRemove(key).Apply(groupKey =>
				{
					_groupKeyToKeyAssociationLookup.TryGetValue(groupKey).Apply(x =>
					{
						x.TryRemove(key);
						if (!x.Any())
						{
							_cancellationTokenSourceLookupByGroupKey.TryRemove(groupKey).Apply(cts => cts.Cancel());
							_groupKeyToKeyAssociationLookup.TryRemove(groupKey);
						}
					});
				});

				return Unit.Value;
			});
		}

		/// <summary>
		/// Removes a subset of data from the cache.
		/// </summary>
		/// <param name="groupKey">The key used to identify a group of cached items.</param>
		public Result<Unit, Exception> RemoveGroup(string groupKey)
		{
			return Result.Try(() =>
			{
				_groupKeyToKeyAssociationLookup.TryRemove(groupKey).Apply(associatedKeys => associatedKeys.Keys.Apply(key => _cancellationTokenSourceLookupByKey.TryRemove(key).Apply(cts => cts.Cancel())));
				_cancellationTokenSourceLookupByGroupKey.TryRemove(groupKey).Apply(cts => cts.Cancel());

				return Unit.Value;
			});
		}

		/// <summary>
		/// Removes all items from the cache.
		/// </summary>
		public Result<Unit, Exception> Clear()
		{
			return Result.Try(() =>
			{
				_cancellationTokenSourceLookupByKey.Apply(x =>
				{
					x.Value.Cancel();
					x.Value.Dispose();
				});

				_groupKeyToKeyAssociationLookup.Clear();
				_cancellationTokenSourceLookupByKey.Clear();
				_cancellationTokenSourceLookupByGroupKey.Clear();

				return Unit.Value;
			});
		}

		/// <summary>
		/// Gets the number of items in the cache.
		/// </summary>
		public int ItemCount => _cache.Count;

		/// <summary>
		/// Gets the number of cancellation tokens associated with cache item keys.
		/// </summary>
		public int CancellationTokenCountByKey => _cancellationTokenSourceLookupByKey.Count;

		/// <summary>
		/// Gets the number of cancellation tokens associated with cache item group keys.
		/// </summary>
		public int CancellationTokenCountByGroupKey => _cancellationTokenSourceLookupByGroupKey.Count;

		private object NullIfNotSpecifiedType(object value, Type type) => value.GetType() == type ? value : null;

		private Option<T> TryGetItem<T>(string key) => _cache.Get(key) is T item ? Option.Some(item) : Option.None<T>();

		private Option<object> TryGetItem(string key, Type type)
		{
			var item = _cache.Get(key);
			return Option.Create((item != null) && (item.GetType() == type), () => item);
		}

		private Result<T, Exception> ExecuteDataRetrieval<T>(string key, Option<string> groupKey, Func<T> dataRetriever, Func<T, bool> shouldCacheData, Func<T, T> dataTransformer, TimeSpan timeToLive) where T : class
		{
			// block retrieval if it is already being executed
			using (var semaphore = new CachedSemaphoreDisposable(_semaphoreSlimLookup, key))
			{
				semaphore.Wait();
				return TryGetItem<T>(key).Match(
					existingItem => Result.Success<T, Exception>(existingItem),
					() => AnalyzeItemAndAddToCacheIfAppropriate(key, groupKey, shouldCacheData, timeToLive, dataRetriever(), dataTransformer));
			}
		}

		private async Task<Result<T, Exception>> ExecuteAsyncDataRetrieval<T>(string key, Option<string> groupKey, Func<Task<T>> dataRetriever, Func<T, bool> shouldCacheData, Func<T, T> dataTransformer, TimeSpan timeToLive) where T : class
		{
			// block retrieval if it is already being executed
			using (var semaphore = new CachedSemaphoreDisposable(_semaphoreSlimLookup, key))
			{
				await semaphore.WaitAsync();
				return await TryGetItem<T>(key).Match(
					existingItem => Task.FromResult(Result.Success<T, Exception>(existingItem)),
					async () => AnalyzeItemAndAddToCacheIfAppropriate(key, groupKey, shouldCacheData, timeToLive, await dataRetriever(), dataTransformer));
			}
		}

		private Result<T, Exception> AnalyzeItemAndAddToCacheIfAppropriate<T>(string key, Option<string> groupKey, Func<T, bool> shouldCacheData, TimeSpan timeToLive, T itemToAdd, Func<T, T> dataTransformer) where T : class
		{
			return shouldCacheData(itemToAdd)
				? Add(key, groupKey, itemToAdd, timeToLive).Select(_ => dataTransformer(itemToAdd))
				: Result.Success<T, Exception>(dataTransformer(itemToAdd));
		}

		private void DoPostEvictionCleanup(object key, object value, EvictionReason reason, object state)
		{
			var keyAsString = key.ToString();
			_cancellationTokenSourceLookupByKey.TryRemove(keyAsString);
			_keyToGroupKeyAssociationLookup.TryRemove(keyAsString).Apply(groupKey =>
			{
				_groupKeyToKeyAssociationLookup.TryGetValue(groupKey).Apply(x => x.TryRemove(keyAsString));
				_cancellationTokenSourceLookupByGroupKey.TryRemove(groupKey);
			});
		}

		private class CachedSemaphoreDisposable : IDisposable
		{
			private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreSlimLookup;
			private readonly string _key;
			private readonly SemaphoreSlim _semaphore;

			public CachedSemaphoreDisposable(ConcurrentDictionary<string, SemaphoreSlim> semaphoreSlimLookup, string key)
			{
				_semaphoreSlimLookup = semaphoreSlimLookup;
				_key = key;
				_semaphore = _semaphoreSlimLookup.GetOrAdd(_key, _ => new SemaphoreSlim(1));
			}

			public void Wait() => _semaphore.Wait();
			public async Task WaitAsync() => await _semaphore.WaitAsync();

			public void Dispose()
			{
				_semaphore.Release();
				_semaphoreSlimLookup.TryRemove(_key);
			}
		}
	}

	internal static class EnumerableExtensions
	{
		public static void Apply<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
				action.Invoke(item);
		}
	}
}
