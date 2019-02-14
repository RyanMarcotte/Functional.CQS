using System;
using System.Collections.Generic;
using System.Linq;
using Functional;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.Caching.Invalidation;
using Functional.CQS.AOP.IoC.PureDI.Caching;
using Functional.CQS.AOP.IoC.PureDI.Caching.Invalidation;
using Functional.CQS.AOP.IoC.SimpleInjector.Caching.Configuration;
using Functional.CQS.AOP.IoC.SimpleInjector.Caching.NullImplementations;
using Functional.CQS.AOP.IoC.SimpleInjector.DecoratorRegistrationGateways;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;
using SimpleInjector;

// ReSharper disable once CheckNamespace
namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching
{
	/// <summary>
	/// Extension methods for <see cref="ConventionBasedDecoratorRegistrationGateway"/>.
	/// </summary>
	public static class ConventionBasedDecoratorRegistrationGatewayExtensions
	{
		/// <summary>
		/// Register all core components required for applying caching decorators to <see cref="IQueryHandler{TQuery,TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementations.
		/// Only handlers with corresponding <see cref="IQueryResultCachingStrategy{TQuery,TResult}"/> implementations will have the caching decorator applied to them.
		/// </summary>
		/// <typeparam name="TCache">The cache type.  Will be registered as singleton.</typeparam>
		/// <param name="gateway">The gateway.</param>
		/// <param name="cacheFactory">The factory method that will be invoked once to create a singleton instance of <typeparamref name="TCache"/>.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		/// <returns></returns>
		public static ConventionBasedDecoratorRegistrationGateway WithCachingDecorator<TCache>(this ConventionBasedDecoratorRegistrationGateway gateway, Func<TCache> cacheFactory, CachingModuleConfigurationParameters configurationParameters)
			where TCache : class, IFunctionalCache
		{
			return gateway.WithCachingDecorator<TCache, NullCacheHitAndMissLogger, NullCacheItemReplacementLogger, NullCacheItemInvalidationLogger, NullCacheExceptionLogger>(cacheFactory, configurationParameters);
		}

		/// <summary>
		/// Register all core components required for applying caching decorators to <see cref="IQueryHandler{TQuery, TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementations.
		/// Only handlers with corresponding <see cref="IQueryResultCachingStrategy{TQuery, TResult}"/> implementations will have the caching decorator applied to them.
		/// </summary>
		/// <typeparam name="TCache">The cache type.  Will be registered as singleton.</typeparam>
		/// <typeparam name="TCacheHitAndMissLogger">The logger for cache hits and misses.  Will be registered as singleton.</typeparam>
		/// <typeparam name="TCacheItemReplacementOperationLogger">The logger for cache item replacement operations.  Will be registered as singleton.</typeparam>
		/// <typeparam name="TCacheItemInvalidationOperationLogger">The logger for cache item invalidation operations.  Will be registered as singleton.</typeparam>
		/// <typeparam name="TCacheExceptionLogger">The logger for cache exceptions.  Will be registered as singleton.</typeparam>
		/// <param name="gateway">The gateway.</param>
		/// <param name="cacheFactory">The factory method that will be invoked once to create a singleton instance of <typeparamref name="TCache"/>.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		public static ConventionBasedDecoratorRegistrationGateway WithCachingDecorator<TCache, TCacheHitAndMissLogger, TCacheItemReplacementOperationLogger, TCacheItemInvalidationOperationLogger, TCacheExceptionLogger>(this ConventionBasedDecoratorRegistrationGateway gateway, Func<TCache> cacheFactory, CachingModuleConfigurationParameters configurationParameters)
			where TCache : class, IFunctionalCache
			where TCacheHitAndMissLogger : class, ILogFunctionalCacheHitsAndMisses
			where TCacheItemReplacementOperationLogger : class, ILogFunctionalCacheItemReplacementOperations
			where TCacheItemInvalidationOperationLogger : class, ILogFunctionalCacheItemInvalidationOperations
			where TCacheExceptionLogger : class, ILogFunctionalCacheExceptions
		{
			if (cacheFactory == null) throw new ArgumentNullException(nameof(cacheFactory));
			if (configurationParameters == null) throw new ArgumentNullException(nameof(configurationParameters));

			gateway.Container.RegisterInstance(configurationParameters);

			gateway.Container.RegisterSingleton<IFunctionalCache>(cacheFactory);
			gateway.Container.RegisterSingleton<ILogFunctionalCacheHitsAndMisses, TCacheHitAndMissLogger>();
			gateway.Container.RegisterSingleton<ILogFunctionalCacheItemReplacementOperations, TCacheItemReplacementOperationLogger>();
			gateway.Container.RegisterSingleton<ILogFunctionalCacheItemInvalidationOperations, TCacheItemInvalidationOperationLogger>();
			gateway.Container.RegisterSingleton<ILogFunctionalCacheExceptions, TCacheExceptionLogger>();
			gateway.Container.RegisterSingleton<IInvalidateFunctionalCacheItems, FunctionalCacheItemInvalidator>();
			gateway.Container.RegisterSingleton(typeof(IInvalidateFunctionalCacheItem<,>), typeof(FunctionalCacheItemInvalidator<,>));
			gateway.Container.RegisterSingleton(typeof(IReplaceFunctionalCacheItem<,>), typeof(FunctionalCacheItemOverwriter<,>));

			if (!configurationParameters.QueryResultCachingDecoratorEnabled)
				return gateway;

			var queryAndResultTypeWithCachingStrategyDefinedCollection = new HashSet<QueryAndResultType>(gateway.AssemblyCollection
				.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsCachingStrategyForQueryType()))
				.Select(x => x.GetGenericParametersForQueryCachingStrategyType())
				.WhereSome());

			bool returnsValueTypeAndHasCachingStrategyDefined(DecoratorPredicateContext c) => c.ToServiceAndImplementationType().ReturnsValueTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection);
			bool returnsReferenceTypeAndHasCachingStrategyDefined(DecoratorPredicateContext c) => c.ToServiceAndImplementationType().ReturnsReferenceTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection);

			gateway.Container.RegisterSingleton(typeof(IQueryResultCachingStrategy<,>), gateway.AssemblyCollection);
			gateway.Container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerCachingDecoratorForValueResultType<,>), gateway.Lifestyle, returnsValueTypeAndHasCachingStrategyDefined);
			gateway.Container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerCachingDecoratorForReferenceResultType<,>), gateway.Lifestyle, returnsReferenceTypeAndHasCachingStrategyDefined);
			gateway.Container.RegisterDecorator(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerCachingDecoratorForValueResultType<,>), gateway.Lifestyle, returnsValueTypeAndHasCachingStrategyDefined);
			gateway.Container.RegisterDecorator(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerCachingDecoratorForReferenceResultType<,>), gateway.Lifestyle, returnsReferenceTypeAndHasCachingStrategyDefined);

			return gateway;
		}
	}
}