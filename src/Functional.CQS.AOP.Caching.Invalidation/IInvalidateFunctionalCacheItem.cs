using System;
using Functional;
using Functional.CQS;

namespace IQ.Vanilla.CQS.AOP.Caching.Invalidation
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Interface for an object that invalidates a specific cache item in the query result caching system.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The type of data returned from the query.</typeparam>
	public interface IInvalidateFunctionalCacheItem<in TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		/// <summary>
		/// Invalidate the cache item associated with the specified parameters.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		Result<Unit, Exception> InvalidateCacheItem(TQuery query);
	}
}
