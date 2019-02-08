using System;

namespace Functional.CQS.AOP.MetricsCapturing
{
	/// <summary>
	/// Interface for a metrics capturing strategy that is applied to all IQ.Vanilla.CQS handlers.
	/// </summary>
	public interface IUniversalMetricsCapturingStrategy
    {
		/// <summary>
		/// Called immediately prior to handling the query or command.
		/// </summary>
		void OnInvocationStart();

		/// <summary>
		/// Called immediately after successfully handling the query or command.
		/// </summary>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		void OnInvocationCompletedSuccessfully(TimeSpan timeElapsed);

		/// <summary>
		/// Called immediately after the query or command failed due to an uncaught exception.
		/// </summary>
		/// <param name="exception">The exception that caused the handler to fail.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		void OnInvocationException(Exception exception, TimeSpan timeElapsed);
	}
}
