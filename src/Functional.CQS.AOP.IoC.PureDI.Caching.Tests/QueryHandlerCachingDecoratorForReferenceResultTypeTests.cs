using System;
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
	public class QueryHandlerCachingDecoratorForReferenceResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		[SuffixFactoryDoesNotProduceSuffix]
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
		[DecoratorIsDisabled]
		public void DoesNotExecuteAnyDecorationCodeIfDecoratorIsDisabled(
			QueryHandlerCachingDecoratorForReferenceResultType<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> sut,
			IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyQueryReturnsReferenceType();
			sut.Handle(query);
			A.CallTo(() => queryHandler.Handle(A<DummyQueryReturnsReferenceType>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyQueryReturnsReferenceType), typeof(DummyQueryReturnsReferenceTypeResult), A<string>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase(Func<Option<string>> suffixFactory, Action<IFunctionalCache, IFunctionalCacheKeySuffixFactory> setupAction, bool decoratorEnabled)
				: base(() => new Fixture()
					.Customize(new QueryHandlerCustomization<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>(() => new DummyQueryReturnsReferenceTypeCachingStrategy()))
					.Customize(new CacheCustomization(setupAction))
					.Customize(new CacheLoggerCustomization())
					.Customize(new CachingModuleConfigurationParametersCustomization(new Configuration.CachingModuleConfigurationParameters(decoratorEnabled))))
			{

			}
		}

		private class ItemDoesNotExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public ItemDoesNotExistInCache()
				: base(() => Option.Some(string.Empty), (cache, suffixFactory) => { }, true)
			{
			}
		}

		private class ItemDoesExistInCache : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			private static void AddItemToCache(IFunctionalCache cache, IFunctionalCacheKeySuffixFactory suffixFactory)
			{
				var cacheKey = new DummyQueryReturnsReferenceTypeCachingStrategy().BuildCacheKeyForQueryWithSuffixApplied(suffixFactory, new DummyQueryReturnsReferenceType()).EnsureValue();
				cache.Add(cacheKey, Option.None<string>(), new DataWrapper<DummyQueryReturnsReferenceTypeResult>(new DummyQueryReturnsReferenceTypeResult()), TimeSpan.FromMinutes(1));
			}

			public ItemDoesExistInCache()
				: base(() => Option.Some(string.Empty), AddItemToCache, true)
			{
			}
		}

		private class SuffixFactoryDoesNotProduceSuffix : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public SuffixFactoryDoesNotProduceSuffix()
				: base(Option.None<string>, (cache, suffixFactory) => { }, true)
			{
			}
		}

		private class DecoratorIsDisabled : QueryHandlerCachingDecoratorForReferenceResultTypeTestsArrangementBase
		{
			public DecoratorIsDisabled()
				: base(() => Option.Some(string.Empty), (cache, suffixFactory) => { }, false)
			{
			}
		}

		#endregion
	}
}