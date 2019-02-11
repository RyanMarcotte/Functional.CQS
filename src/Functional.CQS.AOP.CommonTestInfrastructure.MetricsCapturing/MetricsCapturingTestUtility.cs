using System;
using System.Collections.Generic;
using System.Text;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing;

namespace Functional.CQS.AOP.CommonTestInfrastructure.MetricsCapturing
{
	public static class MetricsCapturingTestUtility
	{
		/// <summary>
		/// Gets the collection of Functional.CQS metrics-capturing strategy implementation types associated with elements in <see cref="TestUtility.CQSHandlerContractTypes"/>.
		/// </summary>
		public static IReadOnlyDictionary<Type, Type> MetricsCapturingDecoratorTypeLookupByCQSHandlerContractType { get; } = new Dictionary<Type, Type>()
		{
			{ typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(QueryHandlerMetricsCapturingDecorator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>) },
			{ typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(QueryHandlerMetricsCapturingDecorator<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>), typeof(AsyncQueryHandlerMetricsCapturingDecorator<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>), typeof(AsyncQueryHandlerMetricsCapturingDecorator<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>) },
			{ typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>), typeof(CommandHandlerMetricsCapturingDecorator<DummyCommandThatSucceeds, DummyCommandError>) },
			{ typeof(ICommandHandler<DummyCommandThatFails, DummyCommandError>), typeof(CommandHandlerMetricsCapturingDecorator<DummyCommandThatFails, DummyCommandError>) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>), typeof(AsyncCommandHandlerMetricsCapturingDecorator<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatFails, DummyAsyncCommandError>), typeof(AsyncCommandHandlerMetricsCapturingDecorator<DummyAsyncCommandThatFails, DummyAsyncCommandError>) }
		};

		/// <summary>
		/// Gets the collection of Functional.CQS metrics-capturing strategy implementation types associated with elements in <see cref="TestUtility.CQSHandlerContractTypes"/>.
		/// </summary>
		public static IReadOnlyDictionary<Type, Type> UniversalMetricsCapturingDecoratorTypeLookupByCQSHandlerContractType { get; } = new Dictionary<Type, Type>()
		{
			{ typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(QueryHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>) },
			{ typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(QueryHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>), typeof(AsyncQueryHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>), typeof(AsyncQueryHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>) },
			{ typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>), typeof(CommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyCommandThatSucceeds, DummyCommandError>) },
			{ typeof(ICommandHandler<DummyCommandThatFails, DummyCommandError>), typeof(CommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyCommandThatFails, DummyCommandError>) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>), typeof(AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatFails, DummyAsyncCommandError>), typeof(AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyAsyncCommandThatFails, DummyAsyncCommandError>) }
		};
	}
}
