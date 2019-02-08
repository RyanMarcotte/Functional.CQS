using System;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing
{
	/// <summary>
	/// Decorator for applying metrics-capturing concerns to <see cref="IAsyncQueryHandler{TQuery,TResult}"/>.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public class AsyncQueryHandlerMetricsCapturingDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		private readonly IAsyncQueryHandler<TQuery, TResult> _handler;
		private readonly IMetricsCapturingStrategyForQuery<TQuery, TResult> _strategy;
		private readonly MetricsCapturingModuleConfigurationParameters _configurationParameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryHandlerMetricsCapturingDecorator{TQuery, TResult}"/> class.
		/// </summary>
		/// <param name="handler">The handler to decorate.</param>
		/// <param name="strategy">The metrics-capturing strategy.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		public AsyncQueryHandlerMetricsCapturingDecorator(
			IAsyncQueryHandler<TQuery, TResult> handler,
			IMetricsCapturingStrategyForQuery<TQuery, TResult> strategy,
			MetricsCapturingModuleConfigurationParameters configurationParameters)
		{
			_handler = handler ?? throw new ArgumentNullException(nameof(handler));
			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
			_configurationParameters = configurationParameters ?? throw new ArgumentNullException(nameof(configurationParameters));
		}

		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			if (!_configurationParameters.QuerySpecificMetricsCapturingDecoratorEnabled)
				return await _handler.HandleAsync(query, cancellationToken);

			return await _handler.HandleAsyncWithMetricsCapturing(query, cancellationToken,
				q => _strategy.OnInvocationStart(q),
				(q, result, timeElapsed) => _strategy.OnInvocationCompletedSuccessfully(q, result, timeElapsed),
				(q, exception, timeElapsed) => _strategy.OnInvocationException(q, exception, timeElapsed));
		}
	}
}