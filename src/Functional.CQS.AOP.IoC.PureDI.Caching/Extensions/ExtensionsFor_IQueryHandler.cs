using System;
using Functional.CQS.AOP.Caching.Infrastructure;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="IQueryHandler{TQuery,TResult}"/>.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	internal static class ExtensionsFor_IQueryHandler
	{
		public static TResult LogCacheMissWithNoKeyAndHandle<TQuery, TResult>(this IQueryHandler<TQuery, TResult> queryHandler, TQuery query, ILogFunctionalCacheHitsAndMisses logger)
			where TQuery : IQueryParameters<TResult>
		{
			if (queryHandler == null) throw new ArgumentNullException(nameof(queryHandler));
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			logger.LogCacheMiss(typeof(TQuery), typeof(TResult), Constants.NoKey);
			return queryHandler.Handle(query);
		}
	}
}