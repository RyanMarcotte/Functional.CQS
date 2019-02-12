using System;

namespace Functional.CQS.AOP.Caching.Infrastructure
{
	/// <summary>
	/// Interface for logging cache hits and misses.
	/// </summary>
	public interface ILogFunctionalCacheHitsAndMisses
	{
		/// <summary>
		/// Log a cache hit.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey">The cache key.</param>
		void LogCacheHit(Type queryType, Type resultType, string cacheKey);

		/// <summary>
		/// Log a cache miss.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey"></param>
		void LogCacheMiss(Type queryType, Type resultType, string cacheKey);
	}

	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Extension methods for <see cref="ILogFunctionalCacheHitsAndMisses"/>.
	/// </summary>
	public static class ExtensionsFor_ILogFunctionalCacheHitsAndMisses
	{
		/// <summary>
		/// Log a cache hit or miss.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="isCacheHit">Indicates if we are logging a cache hit or miss.</param>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey">The cache key.</param>
		public static void DoCacheLogging(this ILogFunctionalCacheHitsAndMisses logger, bool isCacheHit, Type queryType, Type resultType, string cacheKey)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (queryType == null) throw new ArgumentNullException(nameof(queryType));
			if (resultType == null) throw new ArgumentNullException(nameof(resultType));
			if (cacheKey == null) throw new ArgumentNullException(nameof(cacheKey));

			if (isCacheHit)
				logger.LogCacheHit(queryType, resultType, cacheKey);
			else
				logger.LogCacheMiss(queryType, resultType, cacheKey);
		}
	}
}