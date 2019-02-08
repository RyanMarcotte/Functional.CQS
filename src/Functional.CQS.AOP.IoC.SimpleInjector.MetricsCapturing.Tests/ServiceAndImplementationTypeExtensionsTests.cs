using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.Tests
{
	public class ServiceAndImplementationTypeExtensionsTests
	{
		[Fact]
		public void ShouldReturnTrueIfMetricsCapturingStrategyExistsForQueryAndValueResultTypePair()
		{
			var queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection = new[] { new QueryAndResultType(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult)) };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(DummyQueryReturnsValueTypeHandler));
			serviceAndImplementationType.HasMetricsCapturingStrategyDefined(queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection).Should().BeTrue();
		}

		[Fact]
		public void ShouldReturnFalseIfNoMetricsCapturingStrategyExistsForQueryAndValueResultTypePair()
		{
			var queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection = new QueryAndResultType[] { };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(DummyQueryReturnsValueTypeHandler));
			serviceAndImplementationType.HasMetricsCapturingStrategyDefined(queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection).Should().BeFalse();
		}

		[Fact]
		public void ShouldReturnTrueIfMetricsCapturingStrategyExistsForQueryAndResultReferenceTypePair()
		{
			var queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection = new[] { new QueryAndResultType(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult)) };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(DummyQueryReturnsReferenceTypeHandler));
			serviceAndImplementationType.HasMetricsCapturingStrategyDefined(queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection).Should().BeTrue();
		}

		[Fact]
		public void ShouldReturnFalseIfNoMetricsCapturingStrategyExistsForQueryAndResultReferenceTypePair()
		{
			var queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection = new QueryAndResultType[] { };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(DummyQueryReturnsReferenceTypeHandler));
			serviceAndImplementationType.HasMetricsCapturingStrategyDefined(queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection).Should().BeFalse();
		}

		[Fact]
		public void ShouldReturnTrueIfMetricsCapturingStrategyExistsForCommandType()
		{
			var commandTypeWithMetricsCapturingStrategyDefinedCollection = new[] { new CommandAndErrorType(typeof(DummyCommandThatSucceeds), typeof(DummyCommandError)) };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>), typeof(DummyCommandHandlerThatSucceeds));
			serviceAndImplementationType.HasMetricsCapturingStrategyDefined(commandTypeWithMetricsCapturingStrategyDefinedCollection).Should().BeTrue();
		}

		[Fact]
		public void ShouldReturnFalseIfNoMetricsCapturingStrategyExistsForCommandType()
		{
			var commandTypeWithMetricsCapturingStrategyDefinedCollection = new CommandAndErrorType[] { };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>), typeof(DummyCommandHandlerThatSucceeds));
			serviceAndImplementationType.HasMetricsCapturingStrategyDefined(commandTypeWithMetricsCapturingStrategyDefinedCollection).Should().BeFalse();
		}
	}
}
