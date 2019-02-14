using System;

namespace Functional.CQS.AOP.Caching.Infrastructure.Invalidation
{
	/// <summary>
	/// Interface for an object that invalidates a subset of data stored in the query result caching system.
	/// </summary>
	public interface IInvalidateFunctionalCacheItems
	{
		/// <summary>
		/// Invalidate all cache items with the specified group key.
		/// </summary>
		/// <param name="groupKey">The key used to identify a cache item group.</param>
		Result<Unit, Exception> InvalidateCacheItemGroup(string groupKey);

		/// <summary>
		/// Invalidate all cache items.
		/// </summary>
		Result<Unit, Exception> InvalidateAllCacheItems();
	}
}
