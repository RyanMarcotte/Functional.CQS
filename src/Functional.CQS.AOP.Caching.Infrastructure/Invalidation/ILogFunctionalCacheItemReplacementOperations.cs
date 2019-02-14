using System;

namespace Functional.CQS.AOP.Caching.Infrastructure.Invalidation
{
	/// <summary>
	/// Used for logging cache item replacement operations.
	/// </summary>
	public interface ILogFunctionalCacheItemReplacementOperations
    {
		/// <summary>
		/// Log a cache item replacement.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey">The cache key.</param>
		void LogCacheItemReplacement(Type queryType, Type resultType, string cacheKey);
	}
}
