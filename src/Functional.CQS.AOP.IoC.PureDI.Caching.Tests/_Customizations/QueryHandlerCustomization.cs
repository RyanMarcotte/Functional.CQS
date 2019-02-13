using System;
using AutoFixture;
using FakeItEasy;
using Functional.CQS.AOP.Caching;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class QueryHandlerCustomization<TQuery, TResult> : ICustomization
		where TQuery : IQueryParameters<TResult>
	{
		private readonly Func<TResult> _resultFactory;
		private readonly Func<IQueryResultCachingStrategy<TQuery, TResult>> _cachingStrategyFactory;

		public QueryHandlerCustomization(
			Func<TResult> resultFactory,
			Func<IQueryResultCachingStrategy<TQuery, TResult>> cachingStrategyFactory)
		{
			_resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
			_cachingStrategyFactory = cachingStrategyFactory ?? throw new ArgumentNullException(nameof(cachingStrategyFactory));
		}

		public void Customize(IFixture fixture)
		{
			var queryHandler = A.Fake<IQueryHandler<TQuery, TResult>>();
			A.CallTo(() => queryHandler.Handle(A<TQuery>._)).ReturnsLazily(_resultFactory);
			fixture.Inject(queryHandler);

			fixture.Inject(_cachingStrategyFactory.Invoke());
		}
	}
}