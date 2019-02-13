using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.Caching;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests
{
	public class AsyncQueryHandlerCachingDecoratorForReferenceResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
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
		[AsyncQueryHandlerReturnsNull]
		public async Task NeverCacheIfQueryHandlerReturnsNull(
			AsyncQueryHandlerCachingDecoratorForReferenceResultType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> sut,
			IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyAsyncQueryReturnsReferenceType();
			await sut.HandleAsync(query);
			await sut.HandleAsync(query);
			A.CallTo(() => queryHandler.HandleAsync(query, A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustHaveHappenedTwiceExactly();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyAsyncQueryReturnsReferenceType), typeof(DummyAsyncQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class AsyncQueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected AsyncQueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase(Action<IFunctionalCache> setupAction, Func<DummyAsyncQueryReturnsReferenceTypeResult> resultFactory)
				: base(() => new Fixture()
					.Customize(new AsyncQueryHandlerCustomization<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>(resultFactory, () => new DummyAsyncQueryReturnsReferenceTypeCachingStrategy()))
					.Customize(new CacheCustomization(setupAction))
					.Customize(new CacheLoggerCustomization()))
			{

			}
		}

		private class ItemDoesNotExistInCache : AsyncQueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public ItemDoesNotExistInCache()
				: base(cache => { }, () => new DummyAsyncQueryReturnsReferenceTypeResult())
			{
			}
		}

		private class ItemDoesExistInCache : AsyncQueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			private static void AddItemToCache(IFunctionalCache cache)
			{
				var cacheKey = new DummyAsyncQueryReturnsReferenceTypeCachingStrategy().BuildCacheKeyForQuery(new DummyAsyncQueryReturnsReferenceType());
				cache.Add(cacheKey, Option.None<string>(), new DummyAsyncQueryReturnsReferenceTypeResult().ToDataWrapper(), TimeSpan.FromMinutes(1));
			}

			public ItemDoesExistInCache()
				: base(AddItemToCache, () => new DummyAsyncQueryReturnsReferenceTypeResult())
			{
			}
		}

		private class AsyncQueryHandlerReturnsNull : AsyncQueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public AsyncQueryHandlerReturnsNull()
				: base(cache => { }, () => null)
			{
			}
		}

		#endregion
	}
}