using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;
using Functional.Primitives.FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.Tests
{
	public class TypeExtensionsTests
	{
		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyQueryReturnsValueTypeCachingStrategy() => VerifyQueryMetricsCapturingStrategyType<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult, DummyQueryReturnsValueTypeMetricsCapturingStrategy>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyQueryReturnsReferenceTypeCachingStrategy() => VerifyQueryMetricsCapturingStrategyType<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult, DummyQueryReturnsReferenceTypeMetricsCapturingStrategy>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyAsyncQueryReturnsValueTypeCachingStrategy() => VerifyQueryMetricsCapturingStrategyType<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult, DummyAsyncQueryReturnsValueTypeMetricsCapturingStrategy>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyAsyncQueryReturnsReferenceTypeCachingStrategy() => VerifyQueryMetricsCapturingStrategyType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult, DummyAsyncQueryReturnsReferenceTypeMetricsCapturingStrategy>();

		[Fact]
		public void ShouldReturnExpectedCommandAndErrorTypeForDummyCommandMetricsCapturingStrategy() => VerifyCommandMetricsCapturingStrategyType<DummyCommandThatSucceeds, DummyCommandError, DummyCommandThatSucceedsMetricsCapturingStrategy>();

		[Fact]
		public void ShouldReturnExpectedCommandAndErrorTypeForDummyAsyncCommandMetricsCapturingStrategy() => VerifyCommandMetricsCapturingStrategyType<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError, DummyAsyncCommandThatSucceedsMetricsCapturingStrategy>();

		private static void VerifyQueryMetricsCapturingStrategyType<TQuery, TResult, TMetricsCapturingStrategy>()
			where TQuery : IQueryParameters<TResult>
			where TMetricsCapturingStrategy : IMetricsCapturingStrategyForQuery<TQuery, TResult>
		{
			typeof(TMetricsCapturingStrategy).GetGenericParametersForQueryMetricsCapturingStrategyType().Should().HaveValue(x =>
			{
				x.QueryType.Should().Be(typeof(TQuery));
				x.ResultType.Should().Be(typeof(TResult));
			});
		}

		private static void VerifyCommandMetricsCapturingStrategyType<TCommand, TError, TMetricsCapturingStrategy>()
			where TCommand : ICommandParameters<TError>
			where TMetricsCapturingStrategy : IMetricsCapturingStrategyForCommand<TCommand, TError>
		{
			typeof(TMetricsCapturingStrategy).GetGenericParametersForCommandMetricsCapturingStrategyType().Should().HaveValue(x =>
			{
				x.CommandType.Should().Be(typeof(TCommand));
				x.ErrorType.Should().Be(typeof(TError));
			});
		}
	}
}
