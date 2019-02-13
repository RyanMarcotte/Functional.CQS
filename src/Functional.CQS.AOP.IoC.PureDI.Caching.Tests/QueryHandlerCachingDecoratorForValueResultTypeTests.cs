using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Xunit;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests
{
	public class QueryHandlerCachingDecoratorForValueResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		[SuffixFactoryDoesNotProduceSuffix]
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

		[Theory]
		[DecoratorIsDisabled]
		public void DoesNotExecuteAnyDecorationCodeIfDecoratorIsDisabled(
			QueryHandlerCachingDecoratorForValueResultType<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyQueryReturnsValueType();
			sut.Handle(query);
			A.CallTo(() => queryHandler.Handle(A<DummyQueryReturnsValueType>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyQueryReturnsValueType), typeof(DummyQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase(Func<Option<string>> suffixFactory, Action<IFunctionalCache, IFunctionalCacheKeySuffixFactory> setupAction, bool decoratorEnabled)
			: base(() => new Fixture()
				.Customize(new QueryHandlerCustomization<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>(() => new DummyQueryReturnsValueTypeCachingStrategy()))
				.Customize(new CacheCustomization(setupAction))
				.Customize(new CacheLoggerCustomization())
				.Customize(new CachingModuleConfigurationParametersCustomization(new Configuration.CachingModuleConfigurationParameters(decoratorEnabled))))
			{

			}
		}

		private class ItemDoesNotExistInCache : QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase
		{
			public ItemDoesNotExistInCache()
				: base(() => Option.Some(string.Empty), (cache, suffixFactory) => { }, true)
			{
			}
		}

		private class ItemDoesExistInCache : QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase
		{
			private static void AddItemToCache(IFunctionalCache cache, IFunctionalCacheKeySuffixFactory suffixFactory)
			{
				var cacheKey = new DummyQueryReturnsValueTypeCachingStrategy().BuildCacheKeyForQueryWithSuffixApplied(suffixFactory, new DummyQueryReturnsValueType()).EnsureValue();
				cache.Add(cacheKey, Option.None<string>(), new DummyQueryReturnsValueTypeResult(), TimeSpan.FromMinutes(1));
			}

			public ItemDoesExistInCache()
				: base(() => Option.Some(string.Empty), AddItemToCache, true)
			{
			}
		}

		private class SuffixFactoryDoesNotProduceSuffix : QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase
		{
			public SuffixFactoryDoesNotProduceSuffix()
				: base(Option.None<string>, (cache, suffixFactory) => { }, true)
			{
			}
		}

		private class DecoratorIsDisabled : QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase
		{
			public DecoratorIsDisabled()
				: base(() => Option.Some(string.Empty), (cache, suffixFactory) => { }, false)
			{
			}
		}

		#endregion
	}
}
