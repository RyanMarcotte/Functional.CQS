using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.Caching;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Extensions;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests
{
	public class QueryHandlerCachingDecoratorForReferenceResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		[NullItemDoesNotExistInCache]
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
		[NullItemDoesExistInCache]
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

		#region Arrangements

		private abstract class QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase(Action<IFunctionalCache> setupAction, Func<DummyQueryReturnsReferenceTypeResult> resultFactory)
				: base(() => new Fixture()
					.Customize(new QueryHandlerCustomization<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>(resultFactory, () => new DummyQueryReturnsReferenceTypeCachingStrategy()))
					.Customize(new MemoryCacheCustomization(setupAction))
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
			public ItemDoesExistInCache()
				: base(AddItemToCache, () => new DummyQueryReturnsReferenceTypeResult())
			{
			}
		}

		private class NullItemDoesNotExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public NullItemDoesNotExistInCache()
				: base(cache => { }, () => null)
			{
			}
		}

		private class NullItemDoesExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public NullItemDoesExistInCache()
				: base(AddItemToCache, () => null)
			{
			}
		}

		private static void AddItemToCache(IFunctionalCache cache)
		{
			var cacheKey = new DummyQueryReturnsReferenceTypeCachingStrategy().BuildCacheKeyForQuery(new DummyQueryReturnsReferenceType());
			cache.Add(cacheKey, Option.None<string>(), new DummyQueryReturnsReferenceTypeResult().ToDataWrapper(), TimeSpan.FromMinutes(1));
		}

		#endregion
	}
}