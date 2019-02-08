using System;

namespace Functional.CQS.AOP.MetricsCapturing
{
	/// <summary>
	/// Marker interface for metrics capturing strategies used by <see cref="ICommandHandler{TCommand,TError}"/> and <see cref="IAsyncCommandHandler{TCommand, TError}"/>.
	/// Used internally by IQ.Vanilla.CQS.AOP.  Implement <see cref="IMetricsCapturingStrategyForCommand{TCommand, TError}"/> instead.
	/// </summary>
	public interface IMetricsCapturingStrategyForCommand
	{
		
	}

	/// <summary>
	/// Interface for metrics capturing strategies used by <see cref="ICommandHandler{TCommand,TError}"/> and <see cref="IAsyncCommandHandler{TCommand, TError}"/>.
	/// </summary>
	/// <typeparam name="TCommand">The command type.</typeparam>
	/// <typeparam name="TError">The error type.</typeparam>
	public interface IMetricsCapturingStrategyForCommand<in TCommand, TError> : IMetricsCapturingStrategyForCommand
		where TCommand : ICommandParameters<TError>
	{
		/// <summary>
		/// Called immediately prior to handling the command.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		void OnInvocationStart(TCommand parameters);

		/// <summary>
		/// Called immediately after successfully handling the query and receiving a result.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		/// <param name="result">The command result.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		void OnInvocationCompletedSuccessfully(TCommand parameters, Result<Unit, TError> result, TimeSpan timeElapsed);

		/// <summary>
		/// Called immediately after the command failed due to an uncaught exception.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		/// <param name="exception">The exception that caused the command handler to fail.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		void OnInvocationException(TCommand parameters, Exception exception, TimeSpan timeElapsed);
	}
}