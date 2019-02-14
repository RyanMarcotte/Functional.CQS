using System;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing
{
	/// <summary>
	/// Decorator for applying universal metrics-capturing concerns to <see cref="IAsyncQueryHandler{TQuery,TResult}"/>.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public class AsyncQueryHandlerMetricsCapturingDecoratorForUniversalStrategy<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		private readonly IAsyncQueryHandler<TQuery, TResult> _handler;
		private readonly IUniversalMetricsCapturingStrategy _strategy;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryHandlerMetricsCapturingDecoratorForUniversalStrategy{TQuery, TResult}"/> class.
		/// </summary>
		/// <param name="handler">The handler to decorate.</param>
		/// <param name="strategy">The metrics-capturing strategy.</param>
		public AsyncQueryHandlerMetricsCapturingDecoratorForUniversalStrategy(
			IAsyncQueryHandler<TQuery, TResult> handler,
			IUniversalMetricsCapturingStrategy strategy)
		{
			_handler = handler ?? throw new ArgumentNullException(nameof(handler));
			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
		}

		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await _handler.HandleAsyncWithMetricsCapturing(query, cancellationToken,
				q => _strategy.OnInvocationStart(),
				(q, result, timeElapsed) => _strategy.OnInvocationCompletedSuccessfully(timeElapsed),
				(q, exception, timeElapsed) => _strategy.OnInvocationException(exception, timeElapsed));
		}
	}
}