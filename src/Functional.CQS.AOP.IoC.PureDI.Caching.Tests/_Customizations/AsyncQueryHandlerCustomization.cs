using System;
using System.Threading;
using AutoFixture;
using FakeItEasy;
using Functional.CQS.AOP.Caching;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class AsyncQueryHandlerCustomization<TQuery, TResult> : ICustomization
		where TQuery : IQueryParameters<TResult>
	{
		private readonly Func<TResult> _resultFactory;
		private readonly Func<IQueryResultCachingStrategy<TQuery, TResult>> _cachingStrategyFactory;

		public AsyncQueryHandlerCustomization(Func<TResult> resultFactory, Func<IQueryResultCachingStrategy<TQuery, TResult>> cachingStrategyFactory)
		{
			_resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
			_cachingStrategyFactory = cachingStrategyFactory ?? throw new ArgumentNullException(nameof(cachingStrategyFactory));
		}

		public void Customize(IFixture fixture)
		{
			var queryHandler = A.Fake<IAsyncQueryHandler<TQuery, TResult>>();
			A.CallTo(() => queryHandler.HandleAsync(A<TQuery>._, A<CancellationToken>._)).ReturnsLazily(_resultFactory);
			fixture.Inject(queryHandler);

			fixture.Inject(_cachingStrategyFactory.Invoke());
		}
	}
}