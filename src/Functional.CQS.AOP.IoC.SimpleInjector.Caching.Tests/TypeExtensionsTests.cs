using FluentAssertions;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.CommonTestInfrastructure.Caching;
using Functional.CQS.AOP.CommonTestInfrastructure.Caching.DummyObjects;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.Primitives.FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching.Tests
{
	public class TypeExtensionsTests
	{
		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyQueryReturnsValueTypeCachingStrategy() => VerifyQueryResultCachingStrategyType<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult, DummyQueryReturnsValueTypeCachingStrategy>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyQueryReturnsReferenceTypeCachingStrategy() => VerifyQueryResultCachingStrategyType<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult, DummyQueryReturnsReferenceTypeCachingStrategy>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyAsyncQueryReturnsValueTypeCachingStrategy() => VerifyQueryResultCachingStrategyType<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult, DummyAsyncQueryReturnsValueTypeCachingStrategy>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyAsyncQueryReturnsReferenceTypeCachingStrategy() => VerifyQueryResultCachingStrategyType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult, DummyAsyncQueryReturnsReferenceTypeCachingStrategy>();

		private static void VerifyQueryResultCachingStrategyType<TQuery, TResult, TCachingStrategy>()
			where TQuery : IQueryParameters<TResult>
			where TCachingStrategy : IQueryResultCachingStrategy<TQuery, TResult>
		{
			typeof(TCachingStrategy).GetGenericParametersForQueryCachingStrategyType().Should().HaveValue(x =>
			{
				x.QueryType.Should().Be(typeof(TQuery));
				x.ResultType.Should().Be(typeof(TResult));
			});
		}

		public class WhenCheckingIfTypeIsCachingStrategyForQueryType
		{
			[Fact]
			public void ShouldReturnTrueForDummyQueryReturnsValueTypeCachingStrategy() => typeof(DummyQueryReturnsValueTypeCachingStrategy).IsCachingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyAsyncQueryReturnsValueTypeCachingStrategy() => typeof(DummyAsyncQueryReturnsValueTypeCachingStrategy).IsCachingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyQueryReturnsReferenceTypeCachingStrategy() => typeof(DummyQueryReturnsReferenceTypeCachingStrategy).IsCachingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnTrueForDummyAsyncQueryReturnsReferenceTypeCachingStrategy() => typeof(DummyAsyncQueryReturnsReferenceTypeCachingStrategy).IsCachingStrategyForQueryType().Should().BeTrue();

			[Fact]
			public void ShouldReturnFalseForAnyOtherType()
			{
				typeof(int).IsCachingStrategyForQueryType().Should().BeFalse();
				typeof(object).IsCachingStrategyForQueryType().Should().BeFalse();
			}
		}
	}
}