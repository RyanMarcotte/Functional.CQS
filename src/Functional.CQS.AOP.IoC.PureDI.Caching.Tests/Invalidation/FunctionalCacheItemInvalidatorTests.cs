using System;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.Caching.Infrastructure.Invalidation;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching.Invalidation;
using Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Functional.Primitives.FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests.Invalidation
{
	public class FunctionalCacheItemInvalidatorTests
    {
		[Theory]
		[CacheItemInvalidatorArrangement]
		internal void ShouldInvalidateCacheItemAssociatedWithQueryParameters(
			FunctionalCacheItemInvalidator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IQueryResultCachingStrategy<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> cachingStrategy,
			IFunctionalCache cache,
			ILogFunctionalCacheItemInvalidationOperations operationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
		{
			var query = new DummyQueryReturnsValueType();
			sut.InvalidateCacheItem(query).Should().BeSuccessful();

			var cacheKey = cachingStrategy.BuildCacheKeyForQuery(query);
			A.CallTo(() => cache.Remove(cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
			A.CallTo(() => operationLogger.LogCacheItemInvalidation(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
			A.CallTo(() => exceptionLogger.LogException(A<Exception>._)).MustNotHaveHappened();
		}

	    [Theory]
	    [FailingCacheArrangement]
	    internal void ShouldLogErrorIfInvalidateCacheItemOperationFails(
		    FunctionalCacheItemInvalidator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
		    ILogFunctionalCacheItemInvalidationOperations operationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
	    {
		    var query = new DummyQueryReturnsValueType();
		    sut.InvalidateCacheItem(query).Should().BeFaulted();

		    A.CallTo(() => operationLogger.LogCacheItemInvalidation(A<Type>._, A<Type>._, A<string>._)).MustNotHaveHappened();
			A.CallTo(() => exceptionLogger.LogException(A<Exception>._)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
		}

		[Theory]
		[CacheItemInvalidatorArrangement]
		internal void ShouldInvalidateCacheItemGroupWithSpecifiedKey(
			FunctionalCacheItemInvalidator sut,
			IFunctionalCache cache,
			ILogFunctionalCacheItemInvalidationOperations operationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
		{
			const string GROUP = "9001";
			sut.InvalidateCacheItemGroup(GROUP).Should().BeSuccessful();

			A.CallTo(() => cache.RemoveGroup(GROUP)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
			A.CallTo(() => operationLogger.LogCacheGroupInvalidation(GROUP)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
			A.CallTo(() => exceptionLogger.LogException(A<Exception>._)).MustNotHaveHappened();
		}

	    [Theory]
	    [FailingCacheArrangement]
	    internal void ShouldLogErrorIfInvalidateCacheItemGroupOperationFails(
		    FunctionalCacheItemInvalidator sut,
		    ILogFunctionalCacheItemInvalidationOperations operationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
	    {
		    const string GROUP = "9001";
		    sut.InvalidateCacheItemGroup(GROUP).Should().BeFaulted();

		    A.CallTo(() => operationLogger.LogCacheGroupInvalidation(A<string>._)).MustNotHaveHappened();
			A.CallTo(() => exceptionLogger.LogException(A<Exception>._)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
		}

		[Theory]
		[CacheItemInvalidatorArrangement]
		internal void ShouldInvalidateAllCacheItems(
			FunctionalCacheItemInvalidator sut,
			IFunctionalCache cache,
			ILogFunctionalCacheItemInvalidationOperations operationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
		{
			sut.InvalidateAllCacheItems().Should().BeSuccessful();

			A.CallTo(() => cache.Clear()).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
			A.CallTo(() => operationLogger.LogCacheInvalidation()).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
			A.CallTo(() => exceptionLogger.LogException(A<Exception>._)).MustNotHaveHappened();
		}

		[Theory]
		[FailingCacheArrangement]
	    internal void ShouldLogErrorIfClearOperationFails(
		    FunctionalCacheItemInvalidator sut,
		    ILogFunctionalCacheItemInvalidationOperations operationLogger,
			ILogFunctionalCacheExceptions exceptionLogger)
	    {
			sut.InvalidateAllCacheItems().Should().BeFaulted();

		    A.CallTo(() => operationLogger.LogCacheInvalidation()).MustNotHaveHappened();
			A.CallTo(() => exceptionLogger.LogException(A<Exception>._)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
	    }

		#region Arrangements

		private class CacheItemInvalidatorTestsArrangementBase : AutoDataAttribute
		{
			protected CacheItemInvalidatorTestsArrangementBase(bool cacheOperationsSucceed)
				: base(() => new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new FakeCacheCustomization(cacheOperationsSucceed))
					.Customize(new CachingStrategyCustomization())
					.Customize(new CacheItemInvalidationLoggerCustomization()))
			{

			}
		}

		private class CacheItemInvalidatorArrangement : CacheItemInvalidatorTestsArrangementBase
		{
			public CacheItemInvalidatorArrangement()
				: base(true)
			{

			}
		}

	    private class FailingCacheArrangement : CacheItemInvalidatorTestsArrangementBase
		{
		    public FailingCacheArrangement()
			    : base(false)
		    {

		    }
	    }

		#endregion

		#region Customizations

		private class CacheItemInvalidationLoggerCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Inject(A.Fake<ILogFunctionalCacheItemInvalidationOperations>());
				fixture.Inject(A.Fake<ILogFunctionalCacheExceptions>());
			}
		}

		#endregion
	}
}
