using System;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.CommonTestInfrastructure.MetricsCapturing.DummyObjects
{
	/// <summary>
	/// Metrics capturing strategy for <see cref="DummyAsyncCommandHandlerThatSucceeds"/>.
	/// </summary>
	public class DummyAsyncCommandThatSucceedsMetricsCapturingStrategy : IMetricsCapturingStrategyForCommand<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>
	{
		/// <summary>
		/// Called immediately prior to handling the command.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		public void OnInvocationStart(DummyAsyncCommandThatSucceeds parameters)
		{
			
		}

		/// <summary>
		/// Called immediately after successfully handling the query and receiving a result.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		/// <param name="result">The command result.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		public void OnInvocationCompletedSuccessfully(DummyAsyncCommandThatSucceeds parameters, Result<Unit, DummyAsyncCommandError> result, TimeSpan timeElapsed)
		{
			Console.WriteLine(result.Match(_ => "succeeded", _ => "failed"));
		}

		/// <summary>
		/// Called immediately after the command failed due to an uncaught exception.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		/// <param name="exception">The exception that caused the command handler to fail.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		public void OnInvocationException(DummyAsyncCommandThatSucceeds parameters, Exception exception, TimeSpan timeElapsed)
		{
			
		}
	}
}