using System;
using System.Collections.Generic;
using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using SimpleInjector;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Tests
{
	public class ContainerExtensionsTests
	{
		private static readonly Type[] _typesToCheck = new[]
		{
			typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>),
			typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>),
			typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>),
			typeof(IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>),
			typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>),
			typeof(IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>)
		};

		[Fact]
		public void ShouldSuccessfullyRegisterAllFunctionalCQSHandlersAsSingleton()
		{
			var container = new Container();
			container.RegisterAllFunctionalCQSHandlers(Lifestyle.Singleton, typeof(CommonTestInfrastructureAssemblyMarker).Assembly);
			container.Verify();

			foreach (var type in _typesToCheck)
				container.GetInstance(type).Should().BeSameAs(container.GetInstance(type));
		}

		[Fact]
		public void ShouldSuccessfullyRegisterAllFunctionalCQSHandlersAsTransient()
		{
			var container = new Container();
			container.RegisterAllFunctionalCQSHandlers(Lifestyle.Transient, typeof(CommonTestInfrastructureAssemblyMarker).Assembly);
			container.Verify();

			foreach (var type in _typesToCheck)
				container.GetInstance(type).Should().NotBeSameAs(container.GetInstance(type));
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		private class ComponentStub
		{
			private readonly IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> _queryHandlerReturningValueType;
			private readonly IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> _queryHandlerReturningReferenceType;
			private readonly IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> _asyncQueryHandlerReturningValueType;
			private readonly IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> _asyncQueryHandlerReturningReferenceType;
			private readonly ICommandHandler<DummyCommandThatSucceeds, DummyCommandError> _commandHandler;
			private readonly IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError> _asyncCommandHandler;

			public ComponentStub(
				IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> queryHandlerReturningValueType,
				IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> queryHandlerReturningReferenceType,
				IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> asyncQueryHandlerReturningValueType,
				IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> asyncQueryHandlerReturningReferenceType,
				ICommandHandler<DummyCommandThatSucceeds, DummyCommandError> commandHandler,
				IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError> asyncCommandHandler)
			{
				_queryHandlerReturningValueType = queryHandlerReturningValueType;
				_queryHandlerReturningReferenceType = queryHandlerReturningReferenceType;
				_asyncQueryHandlerReturningValueType = asyncQueryHandlerReturningValueType;
				_asyncQueryHandlerReturningReferenceType = asyncQueryHandlerReturningReferenceType;
				_commandHandler = commandHandler;
				_asyncCommandHandler = asyncCommandHandler;
			}
		}
	}
}
