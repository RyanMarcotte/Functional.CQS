using System;
using AutoFixture;
using FakeItEasy;
using IQ.Vanilla.CQS.AOP.Caching;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class QueryHandlerCustomization<TQuery, TResult> : ICustomization
		where TQuery : IQuery<TResult>
	{
		private readonly Func<IQueryResultCachingStrategy<TQuery, TResult>> _cachingStrategyFactory;

		public QueryHandlerCustomization(Func<IQueryResultCachingStrategy<TQuery, TResult>> cachingStrategyFactory)
		{
			_cachingStrategyFactory = cachingStrategyFactory ?? throw new ArgumentNullException(nameof(cachingStrategyFactory));
		}

		public void Customize(IFixture fixture)
		{
			fixture.Inject(A.Fake<IQueryHandler<TQuery, TResult>>());
			fixture.Inject(_cachingStrategyFactory.Invoke());
		}
	}
}