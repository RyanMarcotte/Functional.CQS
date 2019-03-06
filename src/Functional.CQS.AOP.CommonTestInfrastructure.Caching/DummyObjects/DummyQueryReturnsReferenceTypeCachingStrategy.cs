using System;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;

namespace Functional.CQS.AOP.CommonTestInfrastructure.Caching.DummyObjects
{
	/// <summary>
	/// 
	/// </summary>
	public class DummyQueryReturnsReferenceTypeCachingStrategy : IQueryResultCachingStrategy<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>
	{
		/// <summary>
		/// The amount of time the item will live in the cache before a refresh is required.
		/// </summary>
		public TimeSpan TimeToLive => TimeSpan.FromMinutes(15);

		/// <summary>
		/// Builds a cache key for the specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		public string BuildCacheKeyForQuery(DummyQueryReturnsReferenceType query) => string.Empty;

		/// <summary>
		/// Builds a key used for identifying groups of related cached items.  If null, the cached item does not belong to a group.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		public Option<string> BuildCacheGroupKeyForQuery(DummyQueryReturnsReferenceType query) => Option.None<string>();

		/// <summary>
		/// Indicates if a particular result should be stored in the cache.
		/// </summary>
		/// <param name="result">The result from executing the query.</param>
		/// <returns></returns>
		public bool ShouldCacheResult(DummyQueryReturnsReferenceTypeResult result) => true;
	}
}