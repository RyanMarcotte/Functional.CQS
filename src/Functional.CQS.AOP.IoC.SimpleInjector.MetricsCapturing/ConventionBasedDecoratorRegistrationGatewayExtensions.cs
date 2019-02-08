using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Vanilla.CQS;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Installation.DecoratorRegistrationGateways;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Installation.Models;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.MetricsCapturing;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.NullImplementations;
using IQ.Vanilla.CQS.AOP.MetricsCapturing;
using EnumerableExtensions = IQ.Vanilla.EnumerableExtensions;

// ReSharper disable once CheckNamespace
namespace SimpleInjector
{
	/// <summary>
	/// Extension methods for <see cref="ConventionBasedDecoratorRegistrationGateway"/>.
	/// </summary>
	public static class ConventionBasedDecoratorRegistrationGatewayExtensions
	{
		/// <summary>
		/// Register all core components required for applying metrics-capturing decorators to IQ.Vanilla.CQS handler implementations.
		/// Only handlers with corresponding <see cref="IMetricsCapturingStrategyForQuery{TQuery, TResult}"/>, <see cref="IMetricsCapturingStrategyForCommand{TCommand}"/>, and <see cref="IMetricsCapturingStrategyForResultCommand{TCommand, TError}"/> implementations will have the handler-specific metrics-capturing decorator applied to them.
		/// </summary>
		/// <param name="gateway">The gateway.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		/// <returns></returns>
		public static ConventionBasedDecoratorRegistrationGateway WithMetricsCapturingDecorator(this ConventionBasedDecoratorRegistrationGateway gateway, MetricsCapturingModuleConfigurationParameters configurationParameters)
		{
			return gateway.WithMetricsCapturingDecorator<NullUniversalMetricsCapturingStrategy>(configurationParameters);
		}

		/// <summary>
		/// Register all core components required for applying metrics-capturing decorators to IQ.Vanilla.CQS handler implementations.
		/// Only handlers with corresponding <see cref="IMetricsCapturingStrategyForQuery{TQuery, TResult}"/>, <see cref="IMetricsCapturingStrategyForCommand{TCommand}"/>, and <see cref="IMetricsCapturingStrategyForResultCommand{TCommand, TError}"/> implementations will have the handler-specific metrics-capturing decorator applied to them.
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
			gateway.RegisterMetricsCapturingDecoratorForIndividualResultCommandHandlerImplementations();
			gateway.RegisterUniversalMetricsCapturingDecorator<TUniversalMetricsCapturingStrategy>(configurationParameters);

			return gateway;
		}

		private static void RegisterMetricsCapturingDecoratorForIndividualQueryHandlerImplementations(this ConventionBasedDecoratorRegistrationGateway gateway)
		{
			var queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection = new HashSet<QueryAndResultType>(EnumerableExtensions.Choose(gateway.AssemblyCollection
					.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsClass && typeof(IMetricsCapturingStrategyForQuery).IsAssignableFrom(t))), x => x.GetGenericParametersForQueryMetricsCapturingStrategyType()));

			bool hasMetricsCapturingStrategyDefinedForQuery(DecoratorPredicateContext c) => c.ToServiceAndImplementationType().HasMetricsCapturingStrategyDefined(queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection);
			gateway.Container.RegisterSingleton(typeof(IMetricsCapturingStrategyForQuery<,>), gateway.AssemblyCollection);
			gateway.Container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForQuery);
			gateway.Container.RegisterDecorator(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForQuery);
		}

		private static void RegisterMetricsCapturingDecoratorForIndividualCommandHandlerImplementations(this ConventionBasedDecoratorRegistrationGateway gateway)
		{
			var commandTypeWithMetricsCapturingStrategyDefinedCollection = new HashSet<Type>(EnumerableExtensions.Choose(gateway.AssemblyCollection
					.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsClass && typeof(IMetricsCapturingStrategyForCommand).IsAssignableFrom(t))), x => x.GetGenericParametersForCommandMetricsCapturingStrategyType()));

			bool hasMetricsCapturingStrategyDefinedForCommand(DecoratorPredicateContext c) => c.ToServiceAndImplementationType().HasMetricsCapturingStrategyDefined(commandTypeWithMetricsCapturingStrategyDefinedCollection);
			gateway.Container.RegisterSingleton(typeof(IMetricsCapturingStrategyForCommand<>), gateway.AssemblyCollection);
			gateway.Container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerMetricsCapturingDecorator<>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForCommand);
			gateway.Container.RegisterDecorator(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerMetricsCapturingDecorator<>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForCommand);
		}

		private static void RegisterMetricsCapturingDecoratorForIndividualResultCommandHandlerImplementations(this ConventionBasedDecoratorRegistrationGateway gateway)
		{
			var commandAndErrorTypeWithCachingStrategyDefinedCollection = new HashSet<CommandAndErrorType>(EnumerableExtensions.Choose(gateway.AssemblyCollection
					.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsClass && typeof(IMetricsCapturingStrategyForResultCommand).IsAssignableFrom(t))), x => x.GetGenericParametersForResultCommandMetricsCapturingStrategyType()));

			bool hasMetricsCapturingStrategyDefinedForResultCommand(DecoratorPredicateContext c) => c.ToServiceAndImplementationType().HasMetricsCapturingStrategyDefined(commandAndErrorTypeWithCachingStrategyDefinedCollection);
			gateway.Container.RegisterSingleton(typeof(IMetricsCapturingStrategyForResultCommand<,>), gateway.AssemblyCollection);
			gateway.Container.RegisterDecorator(typeof(IResultCommandHandler<,>), typeof(ResultCommandHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForResultCommand);
			gateway.Container.RegisterDecorator(typeof(IAsyncResultCommandHandler<,>), typeof(AsyncResultCommandHandlerMetricsCapturingDecorator<,>), gateway.Lifestyle, hasMetricsCapturingStrategyDefinedForResultCommand);
		}

		private static void RegisterUniversalMetricsCapturingDecorator<TUniversalMetricsCapturingStrategy>(this ConventionBasedDecoratorRegistrationGateway gateway, MetricsCapturingModuleConfigurationParameters configurationParameters)
			where TUniversalMetricsCapturingStrategy : class, IUniversalMetricsCapturingStrategy
		{
			gateway.Container.RegisterInstance(configurationParameters);

			gateway.Container.RegisterSingleton<IUniversalMetricsCapturingStrategy, TUniversalMetricsCapturingStrategy>();
			gateway.Container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerMetricsCapturingDecoratorForUniversalStrategy<>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(IResultCommandHandler<,>), typeof(ResultCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
			gateway.Container.RegisterDecorator(typeof(IAsyncResultCommandHandler<,>), typeof(AsyncResultCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<,>), gateway.Lifestyle);
		}
	}
}