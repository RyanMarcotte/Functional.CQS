using System;
using AutoFixture;
using FakeItEasy;
using IQ.Vanilla.CQS.AOP.Caching;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class AsyncQueryHandlerCustomization<TQuery, TResult> : ICustomization
		where TQuery : IQuery<TResult>
	{
		private readonly Func<IQueryResultCachingStrategy<TQuery, TResult>> _cachingStrategyFactory;

		public AsyncQueryHandlerCustomization(Func<IQueryResultCachingStrategy<TQuery, TResult>> cachingStrategyFactory)
		{
			_cachingStrategyFactory = cachingStrategyFactory ?? throw new ArgumentNullException(nameof(cachingStrategyFactory));
		}

		public void Customize(IFixture fixture)
		{
			fixture.Inject(A.Fake<IAsyncQueryHandler<TQuery, TResult>>());
			fixture.Inject(_cachingStrategyFactory.Invoke());
		}
	}
}