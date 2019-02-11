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
			typeof(ICommandHandler<DummyCommandThatFails, DummyCommandError>),
			typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>),
			typeof(IAsyncCommandHandler<DummyAsyncCommandThatFails, DummyAsyncCommandError>)
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
			{ typeof(ICommandHandler<DummyCommandThatFails, DummyCommandError>), typeof(DummyCommandHandlerThatFails) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>), typeof(DummyAsyncCommandHandlerThatSucceeds) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatFails, DummyAsyncCommandError>), typeof(DummyAsyncCommandHandlerThatFails) }
		};
	}
}