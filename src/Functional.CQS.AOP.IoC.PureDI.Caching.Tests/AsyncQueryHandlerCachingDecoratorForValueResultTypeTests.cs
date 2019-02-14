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
	public class AsyncQueryHandlerCachingDecoratorForValueResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		public async Task ExecutesQueryHandlerIfItemDoesNotExistInCache(
			AsyncQueryHandlerCachingDecoratorForValueResultType<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> sut,
			IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyAsyncQueryReturnsValueType();
			await sut.HandleAsync(query);
			A.CallTo(() => queryHandler.HandleAsync(query, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyAsyncQueryReturnsValueType), typeof(DummyAsyncQueryReturnsValueTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyAsyncQueryReturnsValueType), typeof(DummyAsyncQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
		}

		[Theory]
		[ItemDoesExistInCache]
		public async Task DoesNotExecuteQueryHandlerIfItemDoesExistInCache(
			AsyncQueryHandlerCachingDecoratorForValueResultType<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> sut,
			IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyAsyncQueryReturnsValueType();
			await sut.HandleAsync(query);
			A.CallTo(() => queryHandler.HandleAsync(A<DummyAsyncQueryReturnsValueType>._, A<CancellationToken>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyAsyncQueryReturnsValueType), typeof(DummyAsyncQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyAsyncQueryReturnsValueType), typeof(DummyAsyncQueryReturnsValueTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
		}

		#region Arrangements

		private abstract class QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase(Action<IFunctionalCache> setupAction)
				: base(() => new Fixture()
					.Customize(new AsyncQueryHandlerCustomization<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>(() => new DummyAsyncQueryReturnsValueTypeResult(), () => new DummyAsyncQueryReturnsValueTypeCachingStrategy()))
					.Customize(new MemoryCacheCustomization(setupAction))
					.Customize(new CacheLoggerCustomization()))
			{

			}
		}

		private class ItemDoesNotExistInCache : QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase
		{
			public ItemDoesNotExistInCache()
				: base(cache => { })
			{
			}
		}

		private class ItemDoesExistInCache : QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase
		{
			private static void AddItemToCache(IFunctionalCache cache)
			{
				var cacheKey = new DummyAsyncQueryReturnsValueTypeCachingStrategy().BuildCacheKeyForQuery(new DummyAsyncQueryReturnsValueType());
				cache.Add(cacheKey, Option.None<string>(), new DummyAsyncQueryReturnsValueTypeResult(), TimeSpan.FromMinutes(1));
			}

			public ItemDoesExistInCache()
				: base(AddItemToCache)
			{
			}
		}

		#endregion
	}
}