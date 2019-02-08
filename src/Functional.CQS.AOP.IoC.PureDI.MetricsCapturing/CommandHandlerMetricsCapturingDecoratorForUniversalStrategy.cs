using System;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing
{
	/// <summary>
	/// Decorator for applying universal metrics-capturing concerns to <see cref="ICommandHandler{TCommand,TError}"/>.
	/// </summary>
	/// <typeparam name="TCommand">The command type.</typeparam>
	/// <typeparam name="TError">The error type.</typeparam>
	public class CommandHandlerMetricsCapturingDecoratorForUniversalStrategy<TCommand, TError> : ICommandHandler<TCommand, TError>
		where TCommand : ICommandParameters<TError>
	{
		private readonly ICommandHandler<TCommand, TError> _handler;
		private readonly IUniversalMetricsCapturingStrategy _strategy;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandHandlerMetricsCapturingDecoratorForUniversalStrategy{TCommand, TError}"/> class.
		/// </summary>
		/// <param name="handler">The handler to decorate.</param>
		/// <param name="strategy">The metrics-capturing strategy.</param>
		public CommandHandlerMetricsCapturingDecoratorForUniversalStrategy(
			ICommandHandler<TCommand, TError> handler,
			IUniversalMetricsCapturingStrategy strategy)
		{
			_handler = handler ?? throw new ArgumentNullException(nameof(handler));
			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
		}

		/// <summary>
		/// Handle the command.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <returns></returns>
		public Result<Unit, TError> Handle(TCommand command)
		{
			return _handler.HandleWithMetricsCapturing(command,
				c => _strategy.OnInvocationStart(),
				(c, result, timeElapsed) => _strategy.OnInvocationCompletedSuccessfully(timeElapsed),
				(c, exception, timeElapsed) => _strategy.OnInvocationException(exception, timeElapsed));
		}
	}
}