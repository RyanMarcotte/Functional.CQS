using System;
using Functional;
using Functional.CQS;
using Functional.CQS.AOP.MetricsCapturing;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing
{
	/// <summary>
	/// Decorator for applying metrics-capturing concerns to <see cref="ICommandHandler{TCommand,TError}"/>.
	/// </summary>
	/// <typeparam name="TCommand">The command type.</typeparam>
	/// <typeparam name="TError">The error type.</typeparam>
	public class CommandHandlerMetricsCapturingDecorator<TCommand, TError> : ICommandHandler<TCommand, TError>
		where TCommand : ICommandParameters<TError>
	{
		private readonly ICommandHandler<TCommand, TError> _handler;
		private readonly IMetricsCapturingStrategyForCommand<TCommand, TError> _strategy;
		private readonly MetricsCapturingModuleConfigurationParameters _configurationParameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandHandlerMetricsCapturingDecorator{TCommand, TError}"/> class.
		/// </summary>
		/// <param name="handler">The handler to decorate.</param>
		/// <param name="strategy">The metrics-capturing strategy.</param>
		/// <param name="configurationParameters">The configuration parameters.</param>
		public CommandHandlerMetricsCapturingDecorator(
			ICommandHandler<TCommand, TError> handler,
			IMetricsCapturingStrategyForCommand<TCommand, TError> strategy,
			MetricsCapturingModuleConfigurationParameters configurationParameters)
		{
			_handler = handler ?? throw new ArgumentNullException(nameof(handler));
			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
			_configurationParameters = configurationParameters ?? throw new ArgumentNullException(nameof(configurationParameters));
		}

		/// <summary>
		/// Handle the command.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <returns></returns>
		public Result<Unit, TError> Handle(TCommand command)
		{
			if (!_configurationParameters.CommandSpecificMetricsCapturingDecoratorEnabled)
				return _handler.Handle(command);

			return _handler.HandleWithMetricsCapturing(command,
				c => _strategy.OnInvocationStart(c),
				(c, result, timeElapsed) => _strategy.OnInvocationCompletedSuccessfully(c, result, timeElapsed),
				(c, exception, timeElapsed) => _strategy.OnInvocationException(c, exception, timeElapsed));
		}
	}
}