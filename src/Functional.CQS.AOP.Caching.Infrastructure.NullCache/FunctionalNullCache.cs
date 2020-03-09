using System;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.Caching.Infrastructure.NullCache
{
	/// <summary>
	/// Implements the null object pattern for the <see cref="IFunctionalCache"/>.
	/// </summary>
	public class FunctionalNullCache : IFunctionalCache
	{
		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <typeparam name="T">The type of item to add.</typeparam>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		/// <param name="groupKey">The optional key used for identifying the cached item as part of a group.</param>
		/// <param name="item">The item to add.</param>
		/// <param name="timeToLive">The amount of time to keep the item in the cache.</param>
		public Result<Unit, Exception> Add<T>(string key, Option<string> groupKey, T item, TimeSpan timeToLive)
		{
			return Result.Unit<Exception>();
		}

		/// <summary>
		/// Invokes <paramref name="dataRetriever"/> to retrieve the item.
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
			return Result.Success<T, Exception>(dataRetriever.Invoke());
		}

		/// <summary>
		/// Invokes <paramref name="dataRetriever"/> to retrieve the item.
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
			return Result.Success<object, Exception>(dataRetriever.Invoke());
		}

		/// <summary>
		/// Invokes <paramref name="dataRetriever"/> to retrieve the item.
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
			return Result.Success<T, Exception>(await dataRetriever.Invoke());
		}

		/// <summary>
		/// Invokes <paramref name="dataRetriever"/> to retrieve the item.
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
			return Result.Success<object, Exception>(await dataRetriever.Invoke());
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="key">The key used to uniquely identify the cached item.</param>
		public Result<Unit, Exception> Remove(string key)
		{
			return Result.Unit<Exception>();
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="groupKey">The key used to identify a group of cached items.</param>
		public Result<Unit, Exception> RemoveGroup(string groupKey)
		{
			return Result.Unit<Exception>();
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		public Result<Unit, Exception> Clear()
		{
			return Result.Unit<Exception>();
		}
	}
}
