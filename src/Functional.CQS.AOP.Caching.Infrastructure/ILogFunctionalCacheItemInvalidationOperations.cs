using System;

namespace Functional.CQS.AOP.Caching.Infrastructure
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Used for logging cache item invalidation operations.
	/// </summary>
	public interface ILogFunctionalCacheItemInvalidationOperations
	{
		/// <summary>
		/// Log that a cache item has been invalidated.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey">The cache key.</param>
		void LogCacheItemInvalidation(Type queryType, Type resultType, string cacheKey);

		/// <summary>
		/// Log that a cache item group has been invalidated.
		/// </summary>
		/// <param name="groupKey">The key used to identify a cache item group.</param>
		void LogCacheGroupInvalidation(string groupKey);

		/// <summary>
		/// Log that all cache items have been invalidated.
		/// </summary>
		void LogCacheInvalidation();
	}
}