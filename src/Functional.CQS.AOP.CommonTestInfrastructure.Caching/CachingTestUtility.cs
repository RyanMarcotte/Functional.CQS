using System;
using System.Collections.Generic;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching;

namespace Functional.CQS.AOP.CommonTestInfrastructure.Caching
{
	/// <summary>
	/// Contains common test infrastructure for metrics.
	/// </summary>
	public static class CachingTestUtility
	{
		/// <summary>
		/// Gets the collection of Functional.CQS caching strategy implementation types associated with elements in <see cref="TestUtility.CQSHandlerContractTypes"/>.
		/// </summary>
		public static IReadOnlyDictionary<Type, Type> CachingDecoratorTypeLookupByCQSHandlerContractType { get; } = new Dictionary<Type, Type>()
		{
			{ typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(QueryHandlerCachingDecoratorForValueResultType<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>) },
			{ typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(QueryHandlerCachingDecoratorForReferenceResultType<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>), typeof(AsyncQueryHandlerCachingDecoratorForValueResultType<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>) },
			{ typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>), typeof(AsyncQueryHandlerCachingDecoratorForReferenceResultType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>) },
			
			// decorator is not applied to command handlers
			{ typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>), typeof(DummyCommandHandlerThatSucceeds) },
			{ typeof(ICommandHandler<DummyCommandThatFails, DummyCommandError>), typeof(DummyCommandHandlerThatFails) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>), typeof(DummyAsyncCommandHandlerThatSucceeds) },
			{ typeof(IAsyncCommandHandler<DummyAsyncCommandThatFails, DummyAsyncCommandError>), typeof(DummyAsyncCommandHandlerThatFails) }
		};
	}
}
