using System;
using System.Collections.Generic;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;

namespace Functional.CQS.AOP.CommonTestInfrastructure
{
	/// <summary>
	/// Contains common test infrastructure.
	/// </summary>
	public static class TestUtility
	{
		/// <summary>
		/// Gets the collection of Functional.CQS dummy contract types.
		/// </summary>
		public static Type[] CQSHandlerContractTypes { get; } = new[]
		{
			typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>),
			typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>),
			typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>),
			typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>),
			typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>),
			typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>)
		};

		/// <summary>
		/// Gets the collection of Functional.CQS dummy implementation types associated with elements in <see cref="CQSHandlerContractTypes"/>.
		/// </summary>
		public static IReadOnlyDictionary<Type, Type> ImplementationTypeLookupByCQSHandlerContractType { get; } = new Dictionary<Type, Type>()
		{
			{ typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(DummyQueryReturnsValueTypeHandler) },
			{ typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(DummyQueryReturnsReferenceTypeHandler) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>), typeof(DummyAsyncQueryReturnsValueTypeHandler) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>), typeof(DummyAsyncQueryReturnsReferenceTypeHandler) },
			{ typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>), typeof(DummyCommandHandlerThatSucceeds) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>), typeof(DummyAsyncCommandHandlerThatSucceeds) }
		};

		/// <summary>
		/// Gets the collection of Functional.CQS metrics-capturing strategy implementation types associated with elements in <see cref="CQSHandlerContractTypes"/>.
		/// </summary>
		public static IReadOnlyDictionary<Type, Type> MetricsCapturingDecoratorTypeLookupByCQSHandlerContractType { get; } = new Dictionary<Type, Type>()
		{
			{ typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(DummyQueryReturnsValueTypeMetricsCapturingStrategy) },
			{ typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(DummyQueryReturnsReferenceTypeMetricsCapturingStrategy) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>), typeof(DummyAsyncQueryReturnsValueTypeMetricsCapturingStrategy) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>), typeof(DummyAsyncQueryReturnsReferenceTypeMetricsCapturingStrategy) },
			{ typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>), typeof(DummyCommandMetricsCapturingStrategy) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>), typeof(DummyAsyncCommandMetricsCapturingStrategy) }
		};
	}
}