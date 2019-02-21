using System;

namespace Functional.CQS.AOP.Caching
{
	/// <summary>
	/// Interface for cache item factories used for specifying information for the query result caching system.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The type of data returned from the query.</typeparam>
	public interface IQueryResultCachingStrategy<in TQuery, in TResult>
		where TQuery : IQueryParameters<TResult>
	{
		/// <summary>
		/// The amount of time the item will live in the cache before a refresh is required.
		/// </summary>
		TimeSpan TimeToLive { get; }

		/// <summary>
		/// Build a cache key for the specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		string BuildCacheKeyForQuery(TQuery query);

		/// <summary>
		/// Build a key used for identifying groups of related cached items.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		Option<string> BuildCacheGroupKeyForQuery(TQuery query);

		/// <summary>
		/// Indicates if a particular result should be stored in the cache.
		/// </summary>
		/// <param name="result">The result from executing the query.</param>
		/// <returns></returns>
		bool ShouldCacheResult(TResult result);
	}
}
