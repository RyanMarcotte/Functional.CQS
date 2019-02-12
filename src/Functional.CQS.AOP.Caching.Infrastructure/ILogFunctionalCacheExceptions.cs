using System;

namespace Functional.CQS.AOP.Caching.Infrastructure
{
	/// <summary>
	/// Interface for logging any exceptions that occur when interacting with the cache.
	/// </summary>
	public interface ILogFunctionalCacheExceptions
	{
		/// <summary>
		/// Log an exception that occurred while executing a specific query.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey">The cache key.</param>
		/// <param name="exception">The exception.</param>
		void LogException(Type queryType, Type resultType, string cacheKey, Exception exception);

		/// <summary>
		/// Log an exception that occurred while overwriting or invalidating the cache.
		/// </summary>
		/// <param name="exception">The exception.</param>
		void LogException(Exception exception);
	}
}
