using System;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.Caching.Infrastructure;
using IQ.Vanilla.CQS.AOP.Caching.Invalidation;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Invalidation
{
	/// <summary>
	/// Responsible for invalidating groups of cache items or all cache items.
	/// </summary>
	public class FunctionalCacheItemInvalidator : IInvalidateFunctionalCacheItems
	{
		private readonly IFunctionalCache _cache;
		private readonly ILogFunctionalCacheItemInvalidationOperations _invalidationLogger;
		private readonly ILogFunctionalCacheExceptions _exceptionLogger;

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalCacheItemInvalidator"/> class.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <param name="invalidationLogger">The cache invalidation logger.</param>
		/// <param name="exceptionLogger">The cache exception logger.</param>
		public FunctionalCacheItemInvalidator(
			IFunctionalCache cache,
			ILogFunctionalCacheItemInvalidationOperations invalidationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_invalidationLogger = invalidationLogger ?? throw new ArgumentNullException(nameof(invalidationLogger));
			_exceptionLogger = exceptionLogger ?? throw new ArgumentNullException(nameof(exceptionLogger));
		}

		/// <summary>
		/// Invalidate all cache items with the specified group key.
		/// </summary>
		/// <param name="groupKey">The key used to identify a cache item group.</param>
		public Result<Unit, Exception> InvalidateCacheItemGroup(string groupKey)
		{
			var result = _cache.RemoveGroup(groupKey);
			result.Apply(
				_ => _invalidationLogger.LogCacheGroupInvalidation(groupKey),
				exception => _exceptionLogger.LogException(exception));

			return result;
		}

		/// <summary>
		/// Invalidate all cache items.
		/// </summary>
		public Result<Unit, Exception> InvalidateAllCacheItems()
		{
			var result = _cache.Clear();
			result.Apply(
				_ => _invalidationLogger.LogCacheInvalidation(),
				exception => _exceptionLogger.LogException(exception));
			
			return result;
		}
	}

	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Responsible for invalidating specific cache items.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public class FunctionalCacheItemInvalidator<TQuery, TResult> : IInvalidateFunctionalCacheItem<TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		private readonly IFunctionalCache _cache;
		private readonly IQueryResultCachingStrategy<TQuery, TResult> _cachingStrategy;
		private readonly ILogFunctionalCacheItemInvalidationOperations _invalidationLogger;
		private readonly ILogFunctionalCacheExceptions _exceptionLogger;

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalCacheItemInvalidator{TQuery, TResult}"/> class.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <param name="cachingStrategy">The caching strategy.</param>
		/// <param name="invalidationLogger">The cache invalidation logger.</param>
		/// <param name="exceptionLogger">The exception logger.</param>
		public FunctionalCacheItemInvalidator(
			IFunctionalCache cache,
			IQueryResultCachingStrategy<TQuery, TResult> cachingStrategy,
			ILogFunctionalCacheItemInvalidationOperations invalidationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_cachingStrategy = cachingStrategy ?? throw new ArgumentNullException(nameof(cachingStrategy));
			_invalidationLogger = invalidationLogger ?? throw new ArgumentNullException(nameof(invalidationLogger));
			_exceptionLogger = exceptionLogger ?? throw new ArgumentNullException(nameof(exceptionLogger));
		}

		/// <summary>
		/// Invalidate the cache item associated with the specified parameters.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		public Result<Unit, Exception> InvalidateCacheItem(TQuery query)
		{
			var cacheKey = _cachingStrategy.BuildCacheKeyForQuery(query);
			var result = _cache.Remove(cacheKey);
			result.Apply(
				_ => _invalidationLogger.LogCacheItemInvalidation(typeof(TQuery), typeof(TResult), cacheKey),
				exception => _exceptionLogger.LogException(exception));

			return result;
		}
	}
}
