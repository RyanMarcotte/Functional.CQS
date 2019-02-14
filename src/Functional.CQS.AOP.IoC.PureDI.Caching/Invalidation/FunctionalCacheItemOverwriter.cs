using System;
using System.Collections.Concurrent;
using System.Reflection;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.Caching.Invalidation;
using Functional.CQS.AOP.IoC.PureDI.Caching.Models;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Invalidation
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Base class for overwriting existing items in <see cref="IFunctionalCache"/> implementations.
	/// </summary>
	public abstract class FunctionalCacheItemOverwriter
    {
		// Each reference-type TResult needs its own cache-insertion method, obtained via reflection.
		// This lookup of cache-insertion methods can be shared among all CacheItemOverwriter objects, not just those sharing the same &lt;TQuery, TResult&gt; type parameters.
		//
		// According to an excerpt from the JetBrains wiki found on StackOverflow:
		//      "If you need to have a static field shared between instances with different generic arguments, define a non-generic base class to store your static members, then set
		//       your generic type to inherit from this type."
		// 
		// AakashM, JetBrains. (2012, March 12). ReSharper warns: “Static field in generic type”.
		//      Retrieved July 6, 2017, from https://stackoverflow.com/a/9665168.
		
		private static readonly ConcurrentDictionary<QueryAndResultType, MethodInfo> _referenceTypeCacheInsertionMethodLookup = new ConcurrentDictionary<QueryAndResultType, MethodInfo>();
		private static readonly MethodInfo _insertReferenceTypeReplacementValueIntoCacheMethodInfo = typeof(FunctionalCacheItemOverwriter).GetMethod(nameof(InsertReferenceTypeReplacementValueIntoCache), BindingFlags.Static | BindingFlags.NonPublic);

	    /// <summary>
	    /// Removes the item currently associated with the specified <paramref name="cacheKey"/> and inserts a replacement value.
	    /// </summary>
	    /// <typeparam name="TQuery">The query type.</typeparam>
	    /// <typeparam name="TResult">The result type.</typeparam>
	    /// <param name="cache">The cache.</param>
	    /// <param name="cacheKey">The cache key.</param>
	    /// <param name="groupKey">The optional cache group key.</param>
	    /// <param name="replacementValue">The replacement value.</param>
	    /// <param name="timeToLive">The time to live.</param>
	    /// <param name="operationLogger">The replacement operation logger.</param>
	    /// <param name="exceptionLogger">The exception logger.</param>
	    protected static Result<Unit, Exception> RemoveCurrentValueAndInsertReplacementValueIntoCache<TQuery, TResult>(IFunctionalCache cache, string cacheKey, Option<string> groupKey, TResult replacementValue, TimeSpan timeToLive, ILogFunctionalCacheItemReplacementOperations operationLogger, ILogFunctionalCacheExceptions exceptionLogger)
			where TQuery : IQueryParameters<TResult>
		{
			var result = cache.Remove(cacheKey).Bind(_ => typeof(TResult).IsValueType
				? InsertValueTypeReplacementValueIntoCache<TQuery, TResult>(cache, cacheKey, groupKey, replacementValue, timeToLive, operationLogger, exceptionLogger)
				: ExecuteInsertReferenceTypeReplacementValueIntoCacheUsingReflection<TQuery, TResult>(cache, cacheKey, groupKey, replacementValue, timeToLive, operationLogger, exceptionLogger));

			result.Apply(
				_ => operationLogger.LogCacheItemReplacement(typeof(TQuery), typeof(TResult), cacheKey),
				exception => exceptionLogger.LogException(typeof(TQuery), typeof(TResult), cacheKey, exception));

			return result;
		}

		private static Result<Unit, Exception> InsertValueTypeReplacementValueIntoCache<TQuery, TValueTypeResult>(IFunctionalCache cache, string cacheKey, Option<string> groupKey, TValueTypeResult replacementValue, TimeSpan timeToLive, ILogFunctionalCacheItemReplacementOperations operationLogger, ILogFunctionalCacheExceptions exceptionLogger)
			where TQuery : IQueryParameters<TValueTypeResult>
		{
			return cache.Add(cacheKey, groupKey, replacementValue, timeToLive);
		}

		private static Result<Unit, Exception> ExecuteInsertReferenceTypeReplacementValueIntoCacheUsingReflection<TQuery, TReferenceTypeResult>(IFunctionalCache cache, string cacheKey, Option<string> groupKey, TReferenceTypeResult replacementValue, TimeSpan timeToLive, ILogFunctionalCacheItemReplacementOperations operationLogger, ILogFunctionalCacheExceptions exceptionLogger)
			where TQuery : IQueryParameters<TReferenceTypeResult>
		{
			return (Result<Unit, Exception>)_referenceTypeCacheInsertionMethodLookup
				.GetOrAdd(new QueryAndResultType(typeof(TQuery), typeof(TReferenceTypeResult)), x => _insertReferenceTypeReplacementValueIntoCacheMethodInfo.MakeGenericMethod(x.QueryType, x.ResultType))
				.Invoke(null, new object[] { cache, cacheKey, groupKey, replacementValue, timeToLive, operationLogger, exceptionLogger });
		}

		private static Result<Unit, Exception> InsertReferenceTypeReplacementValueIntoCache<TQuery, TReferenceTypeResult>(IFunctionalCache cache, string cacheKey, Option<string> groupKey, TReferenceTypeResult replacementValue, TimeSpan timeToLive, ILogFunctionalCacheItemReplacementOperations operationLogger, ILogFunctionalCacheExceptions exceptionLogger)
			where TQuery : IQueryParameters<TReferenceTypeResult>
			where TReferenceTypeResult : class
		{
			return cache.Add(cacheKey, groupKey, new DataWrapper<TReferenceTypeResult>(replacementValue), timeToLive);
		}

		#region Models

		private struct QueryAndResultType : IEquatable<QueryAndResultType>
		{
			public QueryAndResultType(Type queryType, Type resultType)
			{
				QueryType = queryType ?? throw new ArgumentNullException(nameof(queryType));
				ResultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
			}

			public Type QueryType { get; }
			public Type ResultType { get; }

			public bool Equals(QueryAndResultType other) => (QueryType == other.QueryType) && (ResultType == other.ResultType);

			public override bool Equals(object obj)
			{
				if (obj is null) return false;
				return (obj.GetType() == this.GetType()) && Equals((QueryAndResultType)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((QueryType != null ? QueryType.GetHashCode() : 0) * 397) ^ (ResultType != null ? ResultType.GetHashCode() : 0);
				}
			}
		}

		#endregion
	}

	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Responsible for overwriting existing items in <see cref="IFunctionalCache"/> implementations.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public class FunctionalCacheItemOverwriter<TQuery, TResult> : FunctionalCacheItemOverwriter, IReplaceFunctionalCacheItem<TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		private readonly IFunctionalCache _cache;
		private readonly IQueryResultCachingStrategy<TQuery, TResult> _cachingStrategy;
		private readonly ILogFunctionalCacheItemReplacementOperations _replacementLogger;
		private readonly ILogFunctionalCacheExceptions _exceptionLogger;

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalCacheItemOverwriter{TQuery, TResult}"/> class.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <param name="cachingStrategy">The caching strategy.</param>
		/// <param name="replacementOperationLogger">The replacement operation logger.</param>
		/// <param name="exceptionLogger">The exception logger.</param>
		public FunctionalCacheItemOverwriter(
			IFunctionalCache cache,
			IQueryResultCachingStrategy<TQuery, TResult> cachingStrategy,
			ILogFunctionalCacheItemReplacementOperations replacementOperationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_cachingStrategy = cachingStrategy ?? throw new ArgumentNullException(nameof(cachingStrategy));
			_replacementLogger = replacementOperationLogger ?? throw new ArgumentNullException(nameof(replacementOperationLogger));
			_exceptionLogger = exceptionLogger ?? throw new ArgumentNullException(nameof(exceptionLogger));
		}

		/// <summary>
		/// Invalidates the specified cache item (if it exists), and then add data to the cache using default time-to-live for that cache item.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="result">The result data to store in the cache.</param>
		public Result<Unit, Exception> ReplaceCacheItem(TQuery query, TResult result) => ReplaceCacheItem(query, result, Option.None<TimeSpan>());

		/// <summary>
		/// Invalidates the specified cache item (if it exists), and then add data to the cache.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="result">The result data to store in the cache.</param>
		/// <param name="timeToLive">The length of time that the result data should be cached.</param>
		public Result<Unit, Exception> ReplaceCacheItem(TQuery query, TResult result, TimeSpan timeToLive) => ReplaceCacheItem(query, result, Option.Some(timeToLive));

		private Result<Unit, Exception> ReplaceCacheItem(TQuery query, TResult result, Option<TimeSpan> timeToLiveOverride)
		{
			var cacheKey = _cachingStrategy.BuildCacheKeyForQuery(query);
			var cacheGroupKey = _cachingStrategy.BuildCacheGroupKeyForQuery(query);
			var timeToLive = timeToLiveOverride.ValueOrDefault(_cachingStrategy.TimeToLive);

			return RemoveCurrentValueAndInsertReplacementValueIntoCache<TQuery, TResult>(_cache, cacheKey, cacheGroupKey, result, timeToLive, _replacementLogger, _exceptionLogger);
		}
	}
}
