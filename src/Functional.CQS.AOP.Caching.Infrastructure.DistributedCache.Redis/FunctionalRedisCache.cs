using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.JsonConverters;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Used to cache data remotely in a Redis cache.
	/// Note that objects stored in the cache must be concrete types in order for serialization to work correctly.
	/// </summary>
	public class FunctionalRedisCache : IFunctionalCache
	{
		private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreSlimLookup = new ConcurrentDictionary<string, SemaphoreSlim>();
		private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
		{
			Converters = new List<JsonConverter>
			{
				new OptionJsonConverter(),
				new ResultJsonConverter()
			}
		};

		private readonly IRedisClientsManager _managerPool;
		private readonly FunctionalRedisCacheOptions _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalRedisCache"/> class.
		/// </summary>
		/// <param name="options">The cache configuration options.</param>
		public FunctionalRedisCache(FunctionalRedisCacheOptions options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_managerPool = new RedisManagerPool(_options.ConnectionString);
		}

		/// <summary>
		/// Adds an item to the cache.
		/// </summary>
		/// <typeparam name="T">The type of item to add.</typeparam>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.</param>
		/// <param name="item">The item to add.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		public Result<Unit, Exception> Add<T>(string key, Option<string> groupKey, T item, TimeSpan timeToLive)
		{
			using (var client = _managerPool.GetClient())
			{
				return groupKey.Match(
					gk => client.SetWithGroupKeySafely(key, gk, item, timeToLive, _serializerSettings).Select(_ => Unit.Value),
					() => client.SetSafely(key, item, timeToLive, _serializerSettings).Select(_ => Unit.Value));
			}
		}

		/// <summary>
		/// Indicates if the cache contains the specified key.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <returns></returns>
		public bool Contains(string key)
		{
			using (var client = _managerPool.GetClient())
				return client.ContainsKey(key);
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
			using (var client = _managerPool.GetClient())
			{
				return TryGetItem<T>(client, key).Bind(itemFromCache => itemFromCache.Match(
					item => Result.Success<T, Exception>(item),
					() => ExecuteDataRetrieval(client, key, groupKey, dataRetriever, shouldCacheData, x => x, timeToLive)));
			}
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
			using (var client = _managerPool.GetClient())
			{
				return TryGetItem(client, key, type).Bind(itemFromCache => itemFromCache.Match(
					item => Result.Success<object, Exception>(item),
					() => ExecuteDataRetrieval(client, key, groupKey, dataRetriever, shouldCacheData, x => NullIfNotSpecifiedType(x, type), timeToLive)));
			}
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
			using (var client = _managerPool.GetClient())
			{
				return await TryGetItem<T>(client, key).BindAsync(itemFromCache => itemFromCache.MatchAsync(
					async item => await Task.FromResult(Result.Success<T, Exception>(item)),
					async () => await ExecuteAsyncDataRetrieval(client, key, groupKey, dataRetriever, shouldCacheData, x => x, timeToLive)));
			}
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
			using (var client = _managerPool.GetClient())
			{
				return await TryGetItem(client, key, type).BindAsync(itemFromCache => itemFromCache.MatchAsync(
					async item => await Task.FromResult(Result.Success<object, Exception>(item)),
					async () => await ExecuteAsyncDataRetrieval(client, key, groupKey, dataRetriever, shouldCacheData, x => NullIfNotSpecifiedType(x, type), timeToLive)));
			}
		}

		/// <summary>
		/// Removes an item from the cache.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		public Result<Unit, Exception> Remove(string key)
		{
			using (var client = _managerPool.GetClient())
				return client.RemoveSafely(key).Select(_ => Unit.Value);
		}

		/// <summary>
		/// Removes a subset of data from the cache.
		/// </summary>
		/// <param name="groupKey">The key used to identify a group of cached items.</param>
		public Result<Unit, Exception> RemoveGroup(string groupKey)
		{
			using (var client = _managerPool.GetClient())
				return client.RemoveGroupSafely(groupKey).Select(_ => Unit.Value);
		}

		/// <summary>
		/// Removes all items from the cache.
		/// </summary>
		public Result<Unit, Exception> Clear()
		{
			return Result.Try(() =>
			{
				using (var client = _managerPool.GetClient())
					client.FlushAll();

				return Unit.Value;
			});
		}

		/// <summary>
		/// Gets the number of items in the cache.
		/// </summary>
		public int ItemCount
		{
			get
			{
				using (var client = _managerPool.GetClient())
				{
					int keyToGroupKeyItemCount = client.CountKeyToGroupKeyAssociationItems().Match(value => value, ex => throw ex);
					int groupKeySetItemCount = client.CountGroupKeySetItems().Match(value => value, ex => throw ex);
					return (int)client.DbSize - keyToGroupKeyItemCount - groupKeySetItemCount;
				}
			}
		}

		/// <summary>
		/// Gets the number of keys that have group keys associated with them.
		/// </summary>
		public int KeyToGroupKeyItemCount
		{
			get
			{
				using (var client = _managerPool.GetClient())
					return client.CountKeyToGroupKeyAssociationItems().Match(value => value, ex => throw ex);
			}
		}

		/// <summary>
		/// Gets the number of group keys.
		/// </summary>
		public int GroupKeySetItemCount
		{
			get
			{
				using (var client = _managerPool.GetClient())
					return client.CountGroupKeySetItems().Match(value => value, ex => throw ex);
			}
		}

		private Result<Option<T>, Exception> TryGetItem<T>(IRedisClient client, string key)
		{
			return client.ContainsKey(key)
				? Result.Try(() => Option.Some((T)JsonConvert.DeserializeObject(client.GetValue(key), typeof(T), _serializerSettings)))
				: Result.Success<Option<T>, Exception>(Option.None<T>());
		}

		private object NullIfNotSpecifiedType(object value, Type type) => value.GetType() == type ? value : null;

		private Result<Option<object>, Exception> TryGetItem(IRedisClient client, string key, Type type)
		{
			return Result.Try(() => Option.Create(client.ContainsKey(key), () => JsonConvert.DeserializeObject(client.GetValue(key), type, _serializerSettings)));
		}

		private Result<T, Exception> ExecuteDataRetrieval<T>(IRedisClient client, string key, Option<string> groupKey, Func<T> dataRetriever, Func<T, bool> shouldCacheData, Func<T, T> dataTransformer, TimeSpan timeToLive) where T : class
		{
			// block retrieval if it is already being executed
			using (var semaphore = new CachedSemaphoreDisposable(_semaphoreSlimLookup, key))
			{
				semaphore.Wait();
				return TryGetItem<T>(client, key).Bind(itemFromCache => itemFromCache.Match(
					item => Result.Success<T, Exception>(item),
					() => AnalyzeItemAndAddToCacheIfAppropriate(key, groupKey, shouldCacheData, timeToLive, dataRetriever(), dataTransformer)));
			}
		}

		private async Task<Result<T, Exception>> ExecuteAsyncDataRetrieval<T>(IRedisClient client, string key, Option<string> groupKey, Func<Task<T>> dataRetriever, Func<T, bool> shouldCacheData, Func<T, T> dataTransformer, TimeSpan timeToLive) where T : class
		{
			// block retrieval if it is already being executed
			using (var semaphore = new CachedSemaphoreDisposable(_semaphoreSlimLookup, key))
			{
				await semaphore.WaitAsync();
				return await TryGetItem<T>(client, key).BindAsync(itemFromCache => itemFromCache.MatchAsync(
					item => Task.FromResult(Result.Success<T, Exception>(item)),
					async () => AnalyzeItemAndAddToCacheIfAppropriate(key, groupKey, shouldCacheData, timeToLive, await dataRetriever(), dataTransformer)));
			}
		}

		private Result<T, Exception> AnalyzeItemAndAddToCacheIfAppropriate<T>(string key, Option<string> groupKey, Func<T, bool> shouldCacheData, TimeSpan timeToLive, T itemToAdd, Func<T, T> dataTransformer) where T : class
		{
			return shouldCacheData(itemToAdd)
				? Add(key, groupKey, itemToAdd, timeToLive).Select(_ => dataTransformer(itemToAdd))
				: Result.Success<T, Exception>(dataTransformer(itemToAdd));
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
}
