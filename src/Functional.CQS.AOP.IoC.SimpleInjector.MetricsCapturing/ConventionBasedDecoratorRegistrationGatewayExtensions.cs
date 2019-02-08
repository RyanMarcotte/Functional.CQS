using System;
using System.Collections.Generic;
using System.Linq;
using Functional;
using Functional.CQS;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;
using Functional.CQS.AOP.IoC.SimpleInjector.DecoratorRegistrationGateways;
using Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing;
using Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.NullImplementations;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;
using Functional.CQS.AOP.MetricsCapturing;

// ReSharper disable once CheckNamespace
namespace SimpleInjector
{
	/// <summary>
	/// Extension methods for <see cref="ConventionBasedDecoratorRegistrationGateway"/>.
	/// </summary>
	public static class ConventionBasedDecoratorRegistrationGatewayExtensions
	{
		/// <summary>
		/// Register all core components required for applying metrics-capturing decorators to Functional.CQS handler implementations.
		/// Only handlers with corresponding <see cref="IMetricsCapturingStrategyForQuery{TQuery, TResult}"/> and <see cref="IMetricsCapturingStrategyForCommand{TCommand, TError}"/> implementations will have the handler-specific metrics-capturing decorator applied to them.
		/// </summary>
		/// <param name="gateway">The gateway.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		/// <returns></returns>
		public static ConventionBasedDecoratorRegistrationGateway WithMetricsCapturingDecorator(this ConventionBasedDecoratorRegistrationGateway gateway, MetricsCapturingModuleConfigurationParameters configurationParameters)
		{
			return gateway.WithMetricsCapturingDecorator<NullUniversalMetricsCapturingStrategy>(configurationParameters);
		}

		/// <summary>
		/// Register all core components required for applying metrics-capturing decorators to Functional.CQS handler implementations.
		/// Only handlers with corresponding <see cref="IMetricsCapturingStrategyForQuery{TQuery, TResult}"/> and <see cref="IMetricsCapturingStrategyForCommand{TCommand, TError}"/> implementations will have the handler-specific metrics-capturing decorator applied to them.
		/// All handler implementations will have the universal metrics-capturing decorator applied to them.
		/// </summary>
		/// <typeparam name="TUniversalMetricsCapturingStrategy">The universal metrics capturing strategy type.</typeparam>
		/// <param name="gateway">The gateway.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		/// <returns></returns>
		public static ConventionBasedDecoratorRegistrationGateway WithMetricsCapturingDecorator<TUniversalMetricsCapturingStrategy>(this ConventionBasedDecoratorRegistrationGateway gateway, MetricsCapturingModuleConfigurationParameters configurationParameters)
			where TUniversalMetricsCapturingStrategy : class, IUniversalMetricsCapturingStrategy
		{
			gateway.RegisterMetricsCapturingDecoratorForIndividualQueryHandlerImplementations();
			gateway.RegisterMetricsCapturingDecoratorForIndividualCommandHandlerImplementations();
			gateway.RegisterUniversalMetricsCapturingDecorator<TUniversalMetricsCapturingStrategy>(configurationParameters);

			return gateway;
		}

		private static void RegisterMetricsCapturingDecoratorForIndividualQueryHandlerImplementations(this ConventionBasedDecoratorRegistrationGateway gateway)
		{
			var queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection = new HashSet<QueryAndResultType>(gateway.AssemblyCollection
				.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsClass && typeof(IMetricsCapturingStrategyForQuery).IsAssignableFrom(t)))
				.Select(x => x.GetGenericParametersForQueryMetricsCapturingStrategyType())
				.WhereSome());

			bool hasMetricsCapturingStrategyDefinedForQuery(DecoratorPredicateContext c) => c.ToServiceAndImplementationType().HasMetricsCapturingStrategyDefined(queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection);
			gateway.Container.RegisterSingleton(typeof(IMetricsCapturingStrategyForQuery<,>), gateway.AssemblyCollection);
			gateway.Container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForQuery);
			gateway.Container.RegisterDecorator(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForQuery);
		}

		private static void RegisterMetricsCapturingDecoratorForIndividualCommandHandlerImplementations(this ConventionBasedDecoratorRegistrationGateway gateway)
		{
			var commandAndErrorTypeWithCachingStrategyDefinedCollection = new HashSet<CommandAndErrorType>(gateway.AssemblyCollection
				.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsClass && typeof(IMetricsCapturingStrategyForCommand).IsAssignableFrom(t)))
				.Select(x => x.GetGenericParametersForCommandMetricsCapturingStrategyType())
				.WhereSome());

			bool hasMetricsCapturingStrategyDefinedForCommand(DecoratorPredicateContext c) => c.ToServiceAndImplementationType().HasMetricsCapturingStrategyDefined(commandAndErrorTypeWithCachingStrategyDefinedCollection);
			gateway.Container.RegisterSingleton(typeof(IMetricsCapturingStrategyForCommand<,>), gateway.AssemblyCollection);
			gateway.Container.RegisterDecorator(typeof(ICommandHandler<,>), typeof(CommandHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForCommand);
			gateway.Container.RegisterDecorator(typeof(IAsyncCommandHandler<,>), typeof(AsyncCommandHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForCommand);
		}

		private static void RegisterUniversalMetricsCapturingDecorator<TUniversalMetricsCapturingStrategy>(this ConventionBasedDecoratorRegistrationGateway gateway, MetricsCapturingModuleConfigurationParameters configurationParameters)
			where TUniversalMetricsCapturingStrategy : class, IUniversalMetricsCapturingStrategy
		{
			gateway.Container.RegisterSingleton<IUniversalMetricsCapturingStrategy, TUniversalMetricsCapturingStrategy>();
			gateway.Container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(ICommandHandler<,>), typeof(CommandHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(IAsyncCommandHandler<,>), typeof(AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
		}
	}
}