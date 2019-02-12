using System;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.IoC.PureDI.Caching.Models;

namespace Functional.CQS.AOP.IoC.PureDI.Caching
{
	/// <summary>
	/// Decorator for <see cref="IAsyncQueryHandler{TQuery,TResult}"/> that applies caching concerns.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public class AsyncQueryHandlerCachingDecoratorForReferenceResultType<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
		where TResult : class
	{
		private readonly IAsyncQueryHandler<TQuery, TResult> _queryHandler;
		private readonly IQueryResultCachingStrategy<TQuery, TResult> _cachingStrategy;
		private readonly IFunctionalCache _cache;
		private readonly ILogFunctionalCacheHitsAndMisses _hitAndMissLogger;
		private readonly ILogFunctionalCacheExceptions _exceptionLogger;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryHandlerCachingDecoratorForReferenceResultType{TQuery, TResult}"/> class.
		/// </summary>
		/// <param name="queryHandler">The query handler to decorate.</param>
		/// <param name="cachingStrategy">The caching strategy.</param>
		/// <param name="cache">The cache.</param>
		/// <param name="hitAndMissLogger">The logger for cache hits and misses.</param>
		/// <param name="exceptionLogger">The logger for cache exceptions.</param>
		public AsyncQueryHandlerCachingDecoratorForReferenceResultType(
			IAsyncQueryHandler<TQuery, TResult> queryHandler,
			IQueryResultCachingStrategy<TQuery, TResult> cachingStrategy,
			IFunctionalCache cache,
			ILogFunctionalCacheHitsAndMisses hitAndMissLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
		{
			_queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
			_cachingStrategy = cachingStrategy ?? throw new ArgumentNullException(nameof(cachingStrategy));
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_hitAndMissLogger = hitAndMissLogger ?? throw new ArgumentNullException(nameof(hitAndMissLogger));
			_exceptionLogger = exceptionLogger ?? throw new ArgumentNullException(nameof(exceptionLogger));
		}

		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			var cacheKey = _cachingStrategy.BuildCacheKeyForQuery(query);
			
			// attempt to retrieve data from cache using cache key; if not in cache, execute the handler
			bool isCacheHit = true;
			var groupKey = _cachingStrategy.BuildCacheGroupKeyForQuery(query);
			var result = await _cache.GetAsync(cacheKey, groupKey, async () =>
			{
				isCacheHit = false;
				return new DataWrapper<TResult>(await _queryHandler.HandleAsync(query, cancellationToken));
			}, input => _cachingStrategy.ShouldCacheResult(input.Data), _cachingStrategy.TimeToLive);

			return await result.Match(
				itemFromCache =>
				{
					_hitAndMissLogger.DoCacheLogging(isCacheHit, typeof(TQuery), typeof(TResult), cacheKey);
					return Task.FromResult(itemFromCache.Data);
				},
				async exception =>
				{
					_exceptionLogger.LogException(typeof(TQuery), typeof(TResult), cacheKey, exception);
					return await _queryHandler.LogCacheMissWithNoKeyAndHandleAsync(query, cancellationToken, _hitAndMissLogger);
				});
		}
	}
}