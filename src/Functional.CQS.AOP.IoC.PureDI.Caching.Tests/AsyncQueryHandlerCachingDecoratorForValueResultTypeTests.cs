using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations;
using Xunit;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests
{
	public class AsyncQueryHandlerCachingDecoratorForValueResultTypeTests
	{
		[Theory]
		[ItemDoesNotExistInCache]
		[SuffixFactoryDoesNotProduceSuffix]
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

		[Theory]
		[DecoratorIsDisabled]
		public async Task DoesNotExecuteAnyDecorationCodeIfDecoratorIsDisabled(
			AsyncQueryHandlerCachingDecoratorForValueResultType<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> sut,
			IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult> queryHandler,
			ILogFunctionalCacheHitsAndMisses logger)
		{
			var query = new DummyAsyncQueryReturnsValueType();
			await sut.HandleAsync(query);
			A.CallTo(() => queryHandler.HandleAsync(A<DummyAsyncQueryReturnsValueType>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => logger.LogCacheMiss(typeof(DummyAsyncQueryReturnsValueType), typeof(DummyAsyncQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
			A.CallTo(() => logger.LogCacheHit(typeof(DummyAsyncQueryReturnsValueType), typeof(DummyAsyncQueryReturnsValueTypeResult), A<string>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerCachingDecoratorForValueResultTypeTestsArrangementBase(Func<Option<string>> suffixFactory, Action<IFunctionalCache, IFunctionalCacheKeySuffixFactory> setupAction, bool decoratorEnabled)
				: base(() => new Fixture()
					.Customize(new AsyncQueryHandlerCustomization<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>(() => new DummyAsyncQueryReturnsValueTypeCachingStrategy()))
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
				var cacheKey = new DummyAsyncQueryReturnsValueTypeCachingStrategy().BuildCacheKeyForQueryWithSuffixApplied(suffixFactory, new DummyAsyncQueryReturnsValueType()).EnsureValue();
				cache.Add(cacheKey, Option.None<string>(), new DummyAsyncQueryReturnsValueTypeResult(), TimeSpan.FromMinutes(1));
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