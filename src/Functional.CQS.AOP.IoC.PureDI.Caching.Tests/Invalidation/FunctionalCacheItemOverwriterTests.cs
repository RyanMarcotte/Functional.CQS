using System;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching.Invalidation;
using Functional.CQS.AOP.IoC.PureDI.Caching.Models;
using Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests.Invalidation
{
	// ReSharper disable once InconsistentNaming
	public class FunctionalCacheItemOverwriterTests
	{
		public class AndTimeToLiveIsSpecified
		{
			public class AndResultTypeIsValueType
			{
				[Theory]
				[CacheItemOverwriterArrangement]
				internal void ShouldInvalidateCacheItemAssociatedWithQueryParametersAndThenAddItemToCache(
					FunctionalCacheItemOverwriter<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
					IFunctionalCache cache,
					ILogFunctionalCacheItemReplacementOperations operationLogger,
					ILogFunctionalCacheExceptions exceptionLogger,
					DummyQueryReturnsValueType query,
					IQueryResultCachingStrategy<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> cachingStrategy,
					TimeSpan newTimeToLive)
				{
					var replacementValue = new DummyQueryReturnsValueTypeResult();
					sut.ReplaceCacheItem(query, replacementValue, newTimeToLive);

					var cacheKey = cachingStrategy.BuildCacheKeyForQuery(query);
					var cacheGroupKey = cachingStrategy.BuildCacheGroupKeyForQuery(query);

					A.CallTo(() => cache.Remove(cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => cache.Add(cacheKey, cacheGroupKey, replacementValue, newTimeToLive)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => operationLogger.LogCacheItemReplacement(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustNotHaveHappened();
				}

				[Theory]
				[FailingCacheArrangement]
				internal void ShouldLogErrorIfCacheInvalidationFails(
					FunctionalCacheItemOverwriter<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
					ILogFunctionalCacheExceptions exceptionLogger,
					TimeSpan newTimeToLive)
				{
					sut.ReplaceCacheItem(new DummyQueryReturnsValueType(), new DummyQueryReturnsValueTypeResult(), newTimeToLive);

					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
				}
			}

			public class AndResultTypeIsReferenceType
			{
				[Theory]
				[CacheItemOverwriterArrangement]
				internal void ShouldInvalidateCacheItemAssociatedWithQueryParametersAndThenAddDataWrappedItemToCache(
					FunctionalCacheItemOverwriter<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> sut,
					IFunctionalCache cache,
					ILogFunctionalCacheItemReplacementOperations operationLogger,
					ILogFunctionalCacheExceptions exceptionLogger,
					DummyQueryReturnsReferenceType query,
					IQueryResultCachingStrategy<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> cachingStrategy,
					TimeSpan newTimeToLive)
				{
					var replacementValue = new DummyQueryReturnsReferenceTypeResult();
					sut.ReplaceCacheItem(query, replacementValue, newTimeToLive);

					var cacheKey = cachingStrategy.BuildCacheKeyForQuery(query);
					var cacheGroupKey = cachingStrategy.BuildCacheGroupKeyForQuery(query);
					A.CallTo(() => cache.Remove(cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);

					A.CallTo(() => cache.Add(cacheKey, cacheGroupKey, replacementValue, newTimeToLive)).MustNotHaveHappened();
					A.CallTo(() => cache.Add(cacheKey, cacheGroupKey, A<DataWrapper<DummyQueryReturnsReferenceTypeResult>>._, newTimeToLive)).WhenArgumentsMatch(args => (args[2] as DataWrapper<DummyQueryReturnsReferenceTypeResult>)?.Data == replacementValue).MustHaveHappenedANumberOfTimesMatching(i => i == 1);

					A.CallTo(() => operationLogger.LogCacheItemReplacement(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => operationLogger.LogCacheItemReplacement(typeof(DummyQueryReturnsReferenceType), typeof(DataWrapper<DummyQueryReturnsReferenceTypeResult>), cacheKey)).MustNotHaveHappened();
					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustNotHaveHappened();
				}

				[Theory]
				[FailingCacheArrangement]
				internal void ShouldLogErrorIfCacheInvalidationFails(
					FunctionalCacheItemOverwriter<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> sut,
					ILogFunctionalCacheExceptions exceptionLogger,
					TimeSpan newTimeToLive)
				{
					sut.ReplaceCacheItem(new DummyQueryReturnsReferenceType(), new DummyQueryReturnsReferenceTypeResult(), newTimeToLive);

					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
				}
			}
		}

		public class AndTimeToLiveIsNotSpecified
		{
			public class AndResultTypeIsValueType
			{
				[Theory]
				[CacheItemOverwriterArrangement]
				internal void ShouldInvalidateCacheItemAssociatedWithQueryParametersAndThenAddItemToCache(
					FunctionalCacheItemOverwriter<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
					IFunctionalCache cache,
					ILogFunctionalCacheItemReplacementOperations operationLogger,
					ILogFunctionalCacheExceptions exceptionLogger,
					DummyQueryReturnsValueType query,
					IQueryResultCachingStrategy<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> cachingStrategy)
				{
					var replacementValue = new DummyQueryReturnsValueTypeResult();
					sut.ReplaceCacheItem(query, replacementValue);

					var cacheKey = cachingStrategy.BuildCacheKeyForQuery(query);
					var cacheGroupKey = cachingStrategy.BuildCacheGroupKeyForQuery(query);
					var cacheItemTimeToLive = cachingStrategy.TimeToLive;

					A.CallTo(() => cache.Remove(cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => cache.Add(cacheKey, cacheGroupKey, replacementValue, cacheItemTimeToLive)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => operationLogger.LogCacheItemReplacement(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustNotHaveHappened();
				}

				[Theory]
				[FailingCacheArrangement]
				internal void ShouldLogErrorIfCacheInvalidationFails(
					FunctionalCacheItemOverwriter<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
					ILogFunctionalCacheExceptions exceptionLogger)
				{
					sut.ReplaceCacheItem(new DummyQueryReturnsValueType(), new DummyQueryReturnsValueTypeResult());

					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
				}
			}

			public class AndResultTypeIsReferenceType
			{
				[Theory]
				[CacheItemOverwriterArrangement]
				internal void ShouldInvalidateCacheItemAssociatedWithQueryParametersAndThenAddDataWrappedItemToCache(
					FunctionalCacheItemOverwriter<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> sut,
					IFunctionalCache cache,
					ILogFunctionalCacheItemReplacementOperations operationLogger,
					ILogFunctionalCacheExceptions exceptionLogger,
					DummyQueryReturnsReferenceType query,
					IQueryResultCachingStrategy<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> cachingStrategy)
				{
					var replacementValue = new DummyQueryReturnsReferenceTypeResult();
					sut.ReplaceCacheItem(query, replacementValue);

					var cacheKey = cachingStrategy.BuildCacheKeyForQuery(query);
					var cacheGroupKey = cachingStrategy.BuildCacheGroupKeyForQuery(query);
					var cacheItemTimeToLive = cachingStrategy.TimeToLive;

					A.CallTo(() => cache.Remove(cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);

					A.CallTo(() => cache.Add(cacheKey, cacheGroupKey, replacementValue, cacheItemTimeToLive)).MustNotHaveHappened();
					A.CallTo(() => cache.Add(cacheKey, cacheGroupKey, A<DataWrapper<DummyQueryReturnsReferenceTypeResult>>._, cacheItemTimeToLive)).WhenArgumentsMatch(args => (args[2] as DataWrapper<DummyQueryReturnsReferenceTypeResult>)?.Data == replacementValue).MustHaveHappenedANumberOfTimesMatching(i => i == 1);

					A.CallTo(() => operationLogger.LogCacheItemReplacement(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), cacheKey)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
					A.CallTo(() => operationLogger.LogCacheItemReplacement(typeof(DummyQueryReturnsReferenceType), typeof(DataWrapper<DummyQueryReturnsReferenceTypeResult>), cacheKey)).MustNotHaveHappened();
					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustNotHaveHappened();
				}

				[Theory]
				[FailingCacheArrangement]
				internal void ShouldLogErrorIfCacheInvalidationFails(
					FunctionalCacheItemOverwriter<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> sut,
					ILogFunctionalCacheExceptions exceptionLogger)
				{
					sut.ReplaceCacheItem(new DummyQueryReturnsReferenceType(), new DummyQueryReturnsReferenceTypeResult());

					A.CallTo(() => exceptionLogger.LogException(A<Type>._, A<Type>._, A<string>._, A<Exception>._)).MustHaveHappenedANumberOfTimesMatching(i => i == 1);
				}
			}
		}	

		#region Arrangements

		private abstract class CacheItemOverwriterTestsArrangementBase : AutoDataAttribute
		{
			protected CacheItemOverwriterTestsArrangementBase(bool cacheOperationsSucceed)
				: base(() => new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new FakeCacheCustomization(cacheOperationsSucceed))
					.Customize(new CachingStrategyCustomization())
					.Customize(new CacheItemReplacementLoggerCustomization()))

			{
			}
		}

		private class CacheItemOverwriterArrangement : CacheItemOverwriterTestsArrangementBase
		{
			public CacheItemOverwriterArrangement()
				: base(true)

			{
			}
		}

		private class FailingCacheArrangement : CacheItemOverwriterTestsArrangementBase
		{
			public FailingCacheArrangement()
				: base(false)

			{
			}
		}

		#endregion

		#region Customizations

		private class CacheItemReplacementLoggerCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Inject(A.Fake<ILogFunctionalCacheItemReplacementOperations>());
				fixture.Inject(A.Fake<ILogFunctionalCacheExceptions>());
			}
		}

		#endregion
	}
}
