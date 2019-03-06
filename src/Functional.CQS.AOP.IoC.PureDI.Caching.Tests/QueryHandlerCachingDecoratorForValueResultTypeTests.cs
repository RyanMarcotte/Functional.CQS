using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.Caching.DummyObjects;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests
{
	public class QueryHandlerCachingDecoratorForValueResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		public void ExecutesQueryHandlerIfItemDoesNotExistInCache(
			QueryHandlerCachingDecoratorForValueResultType<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyQueryReturnsValueType();
			sut.Handle(query);
			A.CallTo(() => queryHandler.Handle(query)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
		}

		[Theory]
		[ItemDoesExistInCache]
		public void DoesNotExecuteQueryHandlerIfItemDoesExistInCache(
			QueryHandlerCachingDecoratorForValueResultType<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyQueryReturnsValueType();
			sut.Handle(query);
			A.CallTo(() => queryHandler.Handle(A<DummyQueryReturnsValueType>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
		}

		#region Arrangements

		private abstract class QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase(Action<IFunctionalCache> setupAction)
			: base(() => new Fixture()
				.Customize(new QueryHandlerCustomization<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>(() => new DummyQueryReturnsValueTypeResult(), () => new DummyQueryReturnsValueTypeCachingStrategy()))
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
				var cacheKey = new DummyQueryReturnsValueTypeCachingStrategy().BuildCacheKeyForQuery(new DummyQueryReturnsValueType());
				cache.Add(cacheKey, Option.None<string>(), new DummyQueryReturnsValueTypeResult(), TimeSpan.FromMinutes(1));
			}

			public ItemDoesExistInCache()
				: base(AddItemToCache)
			{
			}
		}

		#endregion
	}
}
