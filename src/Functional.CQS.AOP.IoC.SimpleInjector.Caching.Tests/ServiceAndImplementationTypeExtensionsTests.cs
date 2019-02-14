using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching.Tests
{
	public class ServiceAndImplementationTypeExtensionsTests
	{
		[Fact]
		public void ShouldReturnTrueIfCachingStrategyExistsForQueryAndValueResultTypePair()
		{
			var queryAndResultTypeWithCachingStrategyDefinedCollection = new[] { new QueryAndResultType(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult)) };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(DummyQueryReturnsValueTypeHandler));
			serviceAndImplementationType.ReturnsValueTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeTrue();
			serviceAndImplementationType.ReturnsReferenceTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeFalse();
		}

		[Fact]
		public void ShouldReturnFalseIfNoCachingStrategyExistsForQueryAndValueResultTypePair()
		{
			var queryAndResultTypeWithCachingStrategyDefinedCollection = new QueryAndResultType[] { };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>), typeof(DummyQueryReturnsValueTypeHandler));
			serviceAndImplementationType.ReturnsValueTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeFalse();
			serviceAndImplementationType.ReturnsReferenceTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeFalse();
		}

		[Fact]
		public void ShouldReturnTrueIfCachingStrategyExistsForQueryAndResultReferenceTypePair()
		{
			var queryAndResultTypeWithCachingStrategyDefinedCollection = new[] { new QueryAndResultType(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult)) };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(DummyQueryReturnsReferenceTypeHandler));
			serviceAndImplementationType.ReturnsValueTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeFalse();
			serviceAndImplementationType.ReturnsReferenceTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeTrue();
		}

		[Fact]
		public void ShouldReturnFalseIfNoCachingStrategyExistsForQueryAndResultReferenceTypePair()
		{
			var queryAndResultTypeWithCachingStrategyDefinedCollection = new QueryAndResultType[] { };
			var serviceAndImplementationType = new ServiceAndImplementationType(typeof(IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>), typeof(DummyQueryReturnsReferenceTypeHandler));
			serviceAndImplementationType.ReturnsValueTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeFalse();
			serviceAndImplementationType.ReturnsReferenceTypeAndHasCachingStrategyDefined(queryAndResultTypeWithCachingStrategyDefinedCollection).Should().BeFalse();
		}
	}
}
