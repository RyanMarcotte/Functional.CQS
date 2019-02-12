using System;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.Caching.Infrastructure;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="IAsyncQueryHandler{TQuery,TResult}"/>.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	internal static class ExtensionsFor_IAsyncQueryHandler
	{
		public static async Task<TResult> LogCacheMissWithNoKeyAndHandleAsync<TQuery, TResult>(this IAsyncQueryHandler<TQuery, TResult> queryHandler, TQuery query, CancellationToken cancellationToken, ILogFunctionalCacheHitsAndMisses logger)
			where TQuery : IQueryParameters<TResult>
		{
			if (queryHandler == null) throw new ArgumentNullException(nameof(queryHandler));
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			logger.LogCacheMiss(typeof(TQuery), typeof(TResult), Constants.NoKey);
			return await queryHandler.HandleAsync(query, cancellationToken);
		}
	}
}