using System;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing
{
	/// <summary>
	/// Decorator for applying metrics-capturing concerns to <see cref="IQueryHandler{TQuery,TResult}"/>.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public class QueryHandlerMetricsCapturingDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		private readonly IQueryHandler<TQuery, TResult> _handler;
		private readonly IMetricsCapturingStrategyForQuery<TQuery, TResult> _strategy;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryHandlerMetricsCapturingDecorator{TQuery, TResult}"/> class.
		/// </summary>
		/// <param name="handler">The handler to decorate.</param>
		/// <param name="strategy">The metrics-capturing strategy.</param>
		public QueryHandlerMetricsCapturingDecorator(
			IQueryHandler<TQuery, TResult> handler,
			IMetricsCapturingStrategyForQuery<TQuery, TResult> strategy)
		{
			_handler = handler ?? throw new ArgumentNullException(nameof(handler));
			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
		}

		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		public TResult Handle(TQuery query)
		{
			return _handler.HandleWithMetricsCapturing(query,
				q => _strategy.OnInvocationStart(q),
				(q, result, timeElapsed) => _strategy.OnInvocationCompletedSuccessfully(q, result, timeElapsed),
				(q, exception, timeElapsed) => _strategy.OnInvocationException(query, exception, timeElapsed));
		}
	}
}
