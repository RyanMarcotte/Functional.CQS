namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration
{
	/// <summary>
	/// Encapsulates configuration parameters used by Functional.CQS.AOP.IoC.CastleWindsor.MetricsCapturing components.
	/// </summary>
	public class MetricsCapturingModuleConfigurationParameters
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MetricsCapturingModuleConfigurationParameters"/> class.
		/// </summary>
		/// <param name="universalMetricsCapturingDecoratorEnabled">Indicates if the universal metrics-capturing decorator is enabled.</param>
		/// <param name="commandSpecificMetricsCapturingDecoratorEnabled">Indicates if the metrics-capturing decorator for <see cref="ICommandHandler{TCommand, TError}"/> and <see cref="IAsyncCommandHandler{TCommand, TError}"/> is enabled.</param>
		/// <param name="querySpecificMetricsCapturingDecoratorEnabled">Indicates if the metrics-capturing decorator for <see cref="IQueryHandler{TQuery, TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> is enabled.</param>
		public MetricsCapturingModuleConfigurationParameters(
			bool universalMetricsCapturingDecoratorEnabled,
			bool commandSpecificMetricsCapturingDecoratorEnabled,
			bool querySpecificMetricsCapturingDecoratorEnabled)
		{
			UniversalMetricsCapturingDecoratorEnabled = universalMetricsCapturingDecoratorEnabled;
			CommandSpecificMetricsCapturingDecoratorEnabled = commandSpecificMetricsCapturingDecoratorEnabled;
			QuerySpecificMetricsCapturingDecoratorEnabled = querySpecificMetricsCapturingDecoratorEnabled;
		}

		/// <summary>
		/// Indicates if the universal metrics-capturing decorator is enabled.
		/// </summary>
		public bool UniversalMetricsCapturingDecoratorEnabled { get; }

		/// <summary>
		/// Indicates if the metrics-capturing decorator for <see cref="ICommandHandler{TCommand, TError}"/> and <see cref="IAsyncCommandHandler{TCommand, TError}"/> is enabled.
		/// </summary>
		public bool CommandSpecificMetricsCapturingDecoratorEnabled { get; }

		/// <summary>
		/// Indicates if the metrics-capturing decorator for <see cref="IQueryHandler{TQuery, TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> is enabled.
		/// </summary>
		public bool QuerySpecificMetricsCapturingDecoratorEnabled { get; }
	}
}
