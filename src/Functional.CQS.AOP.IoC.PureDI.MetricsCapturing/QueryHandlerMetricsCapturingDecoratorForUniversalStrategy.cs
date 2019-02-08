using System;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing
{
	/// <summary>
	/// Decorator for applying universal metrics-capturing concerns to <see cref="IQueryHandler{TQuery,TResult}"/>.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public class QueryHandlerMetricsCapturingDecoratorForUniversalStrategy<TQuery, TResult> : IQueryHandler<TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		private readonly IQueryHandler<TQuery, TResult> _handler;
		private readonly IUniversalMetricsCapturingStrategy _strategy;
		private readonly MetricsCapturingModuleConfigurationParameters _configurationParameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryHandlerMetricsCapturingDecoratorForUniversalStrategy{TQuery, TResult}"/> class.
		/// </summary>
		/// <param name="handler">The handler to decorate.</param>
		/// <param name="strategy">The metrics-capturing strategy.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		public QueryHandlerMetricsCapturingDecoratorForUniversalStrategy(
			IQueryHandler<TQuery, TResult> handler,
			IUniversalMetricsCapturingStrategy strategy,
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
		/// <returns></returns>
		public TResult Handle(TQuery query)
		{
			if (!_configurationParameters.UniversalMetricsCapturingDecoratorEnabled)
				return _handler.Handle(query);

			return _handler.HandleWithMetricsCapturing(query,
				q => _strategy.OnInvocationStart(),
				(q, result, timeElapsed) => _strategy.OnInvocationCompletedSuccessfully(timeElapsed),
				(q, exception, timeElapsed) => _strategy.OnInvocationException(exception, timeElapsed));
		}
	}
}