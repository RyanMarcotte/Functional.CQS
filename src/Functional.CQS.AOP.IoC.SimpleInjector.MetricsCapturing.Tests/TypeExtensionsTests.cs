using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.CommonTestInfrastructure.MetricsCapturing.DummyObjects;
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

		public class WhenCheckingIfTypeIsMetricsCapturingStrategyForQueryType
		{
			[Fact]
			public void ShouldReturnTrueForDummyQueryReturnsValueTypeMetricsCapturingStrategy() => typeof(DummyQueryReturnsValueTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyAsyncQueryReturnsValueTypeMetricsCapturingStrategy() => typeof(DummyAsyncQueryReturnsValueTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyQueryReturnsReferenceTypeMetricsCapturingStrategy() => typeof(DummyQueryReturnsReferenceTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyAsyncQueryReturnsReferenceTypeMetricsCapturingStrategy() => typeof(DummyAsyncQueryReturnsReferenceTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnFalseForDummyCommandThatSucceedsMetricsCapturingStrategy() => typeof(DummyCommandThatSucceedsMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeFalse();

			[Fact]
			public void ShouldReturnFalseForDummyAsyncCommandThatSucceedsMetricsCapturingStrategy() => typeof(DummyAsyncCommandThatSucceedsMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeFalse();

			[Fact]
			public void ShouldReturnFalseForDummyCommandThatFailsMetricsCapturingStrategy() => typeof(DummyCommandThatFailsMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeFalse();

			[Fact]
			public void ShouldReturnFalseForDummyAsyncCommandThatFailsMetricsCapturingStrategy() => typeof(DummyAsyncCommandThatFailsMetricsCapturingStrategy).IsMetricsCapturingStrategyForQueryType().Should().BeFalse();

			[Fact]
			public void ShouldReturnFalseForOtherTypes()
			{
				typeof(int).IsMetricsCapturingStrategyForQueryType().Should().BeFalse();
				typeof(object).IsMetricsCapturingStrategyForQueryType().Should().BeFalse();
			}
		}

		public class WhenCheckingIfTypeIsMetricsCapturingStrategyForCommandType
		{
			[Fact]
			public void ShouldReturnFalseForDummyQueryReturnsValueTypeMetricsCapturingStrategy() => typeof(DummyQueryReturnsValueTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeFalse();

			[Fact]
			public void ShouldReturnFalseForDummyAsyncQueryReturnsValueTypeMetricsCapturingStrategy() => typeof(DummyAsyncQueryReturnsValueTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeFalse();

			[Fact]
			public void ShouldReturnFalseForDummyQueryReturnsReferenceTypeMetricsCapturingStrategy() => typeof(DummyQueryReturnsReferenceTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeFalse();

			[Fact]
			public void ShouldReturnFalseForDummyAsyncQueryReturnsReferenceTypeMetricsCapturingStrategy() => typeof(DummyAsyncQueryReturnsReferenceTypeMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeFalse();

			[Fact]
			public void ShouldReturnTrueForDummyCommandThatSucceedsMetricsCapturingStrategy() => typeof(DummyCommandThatSucceedsMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyAsyncCommandThatSucceedsMetricsCapturingStrategy() => typeof(DummyAsyncCommandThatSucceedsMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyCommandThatFailsMetricsCapturingStrategy() => typeof(DummyCommandThatFailsMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyAsyncCommandThatFailsMetricsCapturingStrategy() => typeof(DummyAsyncCommandThatFailsMetricsCapturingStrategy).IsMetricsCapturingStrategyForCommandType().Should().BeTrue();

			[Fact]
			public void ShouldReturnFalseForOtherTypes()
			{
				typeof(int).IsMetricsCapturingStrategyForCommandType().Should().BeFalse();
				typeof(object).IsMetricsCapturingStrategyForCommandType().Should().BeFalse();
			}
		}
	}
}
