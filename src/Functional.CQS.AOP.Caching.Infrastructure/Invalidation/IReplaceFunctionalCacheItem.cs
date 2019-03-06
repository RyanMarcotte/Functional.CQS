using System;

namespace Functional.CQS.AOP.Caching.Invalidation
{
	/// <summary>
	/// Interface for an object that overwrites data already stored in the query result caching system.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The query result type.</typeparam>
	public interface IReplaceFunctionalCacheItem<in TQuery, in TResult>
		where TQuery : IQueryParameters<TResult>
	{
		/// <summary>
		/// Invalidates the specified cache item (if it exists), and then add data to the cache using default time-to-live for that cache item.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="result">The result data to store in the cache.</param>
		Result<Unit, Exception> ReplaceCacheItem(TQuery query, TResult result);

		/// <summary>
		/// Invalidates the specified cache item (if it exists), and then add data to the cache.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="result">The result data to store in the cache.</param>
		/// <param name="timeToLive">The length of time that the result data should be cached.</param>
		Result<Unit, Exception> ReplaceCacheItem(TQuery query, TResult result, TimeSpan timeToLive);
	}
}
