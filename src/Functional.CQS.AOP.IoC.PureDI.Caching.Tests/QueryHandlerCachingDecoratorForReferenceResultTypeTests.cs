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
	public class QueryHandlerCachingDecoratorForReferenceResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		public void ExecutesQueryHandlerIfItemDoesNotExistInCache(
			QueryHandlerCachingDecoratorForReferenceResultType<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> sut,
			IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyQueryReturnsReferenceType();
			sut.Handle(query);
			A.CallTo(() => queryHandler.Handle(query)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
		}

		[Theory]
		[ItemDoesExistInCache]
		public void DoesNotExecuteQueryHandlerIfItemDoesExistInCache(
			QueryHandlerCachingDecoratorForReferenceResultType<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> sut,
			IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyQueryReturnsReferenceType();
			sut.Handle(query);
			A.CallTo(() => queryHandler.Handle(A<DummyQueryReturnsReferenceType>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), A<string>._)).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[QueryHandlerReturnsNull]
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

		private abstract class QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase(Action<IFunctionalCache> setupAction, Func<DummyQueryReturnsReferenceTypeResult> resultFactory)
				: base(() => new Fixture()
					.Customize(new QueryHandlerCustomization<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>(resultFactory, () => new DummyQueryReturnsReferenceTypeCachingStrategy()))
					.Customize(new CacheCustomization(setupAction))
					.Customize(new CacheLoggerCustomization()))
			{

			}
		}

		private class ItemDoesNotExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public ItemDoesNotExistInCache()
				: base(cache => { }, () => new DummyQueryReturnsReferenceTypeResult())
			{
			}
		}

		private class ItemDoesExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			private static void AddItemToCache(IFunctionalCache cache)
			{
				var cacheKey = new DummyQueryReturnsReferenceTypeCachingStrategy().BuildCacheKeyForQuery(new DummyQueryReturnsReferenceType());
				cache.Add(cacheKey, Option.None<string>(), new DummyQueryReturnsReferenceTypeResult(), TimeSpan.FromMinutes(1));
			}

			public ItemDoesExistInCache()
				: base(AddItemToCache, () => new DummyQueryReturnsReferenceTypeResult())
			{
			}
		}

		private class QueryHandlerReturnsNull : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public QueryHandlerReturnsNull()
				: base(cache => { }, () => null)
			{
			}
		}

		#endregion
	}
}