using System;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.Caching.Infrastructure
{
	/// <summary>
	/// Interface for a cache used by Functional.CQS.AOP.Caching components.
	/// </summary>
	public interface IFunctionalCache
	{
		/// <summary>
		/// Adds an item to the cache.
		/// </summary>
		/// <typeparam name="T">The type of item to add.</typeparam>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.</param>
		/// <param name="item">The item to add.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		Result<Unit, Exception> Add<T>(string key, Option<string> groupKey, T item, TimeSpan timeToLive);

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
		Result<T, Exception> Get<T>(string key, Option<string> groupKey, Func<T> dataRetriever, Func<T, bool> shouldCacheData, TimeSpan timeToLive) where T : class;

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
		Result<object, Exception> Get(string key, Option<string> groupKey, Type type, Func<object> dataRetriever, Func<object, bool> shouldCacheData, TimeSpan timeToLive);

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
		Task<Result<T, Exception>> GetAsync<T>(string key, Option<string> groupKey, Func<Task<T>> dataRetriever, Func<T, bool> shouldCacheData, TimeSpan timeToLive) where T : class;

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
		Task<Result<object, Exception>> GetAsync(string key, Option<string> groupKey, Type type, Func<Task<object>> dataRetriever, Func<object, bool> shouldCacheData, TimeSpan timeToLive);

		/// <summary>
		/// Removes an item from the cache.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		Result<Unit, Exception> Remove(string key);

		/// <summary>
		/// Removes a subset of data from the cache.
		/// </summary>
		/// <param name="groupKey">The key used to identify a group of cached items.</param>
		Result<Unit, Exception> RemoveGroup(string groupKey);

		/// <summary>
		/// Removes all items from the cache.
		/// </summary>
		Result<Unit, Exception> Clear();
	}
}
