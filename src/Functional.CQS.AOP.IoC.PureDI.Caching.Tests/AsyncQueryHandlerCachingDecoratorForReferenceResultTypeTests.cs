using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional;
using Functional.CQS;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Xunit;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests
{
	public class AsyncQueryHandlerCachingDecoratorForReferenceResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		[SuffixFactoryDoesNotProduceSuffix]
		public async Task ExecutesQueryHandlerIfItemDoesNotExistInCache(
			AsyncQueryHandlerCachingDecoratorForReferenceResultType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> sut,
			IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyAsyncQueryReturnsReferenceType();
			await sut.HandleAsync(query);
			A.CallTo(() => queryHandler.HandleAsync(query, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
		}

		[Theory]
		[ItemDoesExistInCache]
		public async Task DoesNotExecuteQueryHandlerIfItemDoesExistInCache(
			AsyncQueryHandlerCachingDecoratorForReferenceResultType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> sut,
			IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyAsyncQueryReturnsReferenceType();
			await sut.HandleAsync(query);
			A.CallTo(() => queryHandler.HandleAsync(A<DummyAsyncQueryReturnsReferenceType>._, A<CancellationToken>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[DecoratorIsDisabled]
		public async Task DoesNotExecuteAnyDecorationCodeIfDecoratorIsDisabled(
			AsyncQueryHandlerCachingDecoratorForReferenceResultType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> sut,
			IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyAsyncQueryReturnsReferenceType();
			await sut.HandleAsync(query);
			A.CallTo(() => queryHandler.HandleAsync(A<DummyAsyncQueryReturnsReferenceType>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase(Func<Option<string>> suffixFactory, Action<IFunctionalCache, IFunctionalCacheKeySuffixFactory> setupAction)
				: base(() => new Fixture()
					.Customize(new AsyncQueryHandlerCustomization<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>(() => new DummyAsyncQueryReturnsReferenceTypeCachingStrategy()))
					.Customize(new CacheCustomization(setupAction))
					.Customize(new CacheLoggerCustomization()))
			{

			}
		}

		private class ItemDoesNotExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public ItemDoesNotExistInCache()
				: base(() => Option.Some(string.Empty), (cache, suffixFactory) => { })
			{
			}
		}

		private class ItemDoesExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			private static void AddItemToCache(IFunctionalCache cache, IFunctionalCacheKeySuffixFactory suffixFactory)
			{
				var cacheKey = new DummyAsyncQueryReturnsReferenceTypeCachingStrategy().BuildCacheKeyForQueryWithSuffixApplied(suffixFactory, new DummyAsyncQueryReturnsReferenceType()).EnsureValue();
				cache.Add(cacheKey, Option.None<string>(), new DataWrapper<DummyAsyncQueryReturnsReferenceTypeResult>(new DummyAsyncQueryReturnsReferenceTypeResult()), TimeSpan.FromMinutes(1));
			}

			public ItemDoesExistInCache()
				: base(() => Option.Some(string.Empty), AddItemToCache)
			{
			}
		}

		private class SuffixFactoryDoesNotProduceSuffix : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public SuffixFactoryDoesNotProduceSuffix()
				: base(Option.None<string>, (cache, suffixFactory) => { })
			{
			}
		}

		private class DecoratorIsDisabled : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public DecoratorIsDisabled()
				: base(() => Option.Some(string.Empty), (cache, suffixFactory) => { })
			{
			}
		}

		#endregion
	}
}