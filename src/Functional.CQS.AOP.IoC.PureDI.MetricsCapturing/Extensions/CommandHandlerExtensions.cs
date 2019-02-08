using System;
using System.Diagnostics;
using Functional;
using Functional.CQS;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions
{
	internal static class CommandHandlerExtensions
	{
		public static Result<Unit, TError> HandleWithMetricsCapturing<TCommand, TError>(
			this ICommandHandler<TCommand, TError> handler,
			TCommand command,
			Action<TCommand> onInvocationStart,
			Action<TCommand, Result<Unit, TError>, TimeSpan> onInvocationCompletedSuccessfully,
			Action<TCommand, Exception, TimeSpan> onInvocationException)
			where TCommand : ICommandParameters<TError>
		{
			onInvocationStart.Invoke(command);

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			try
			{
				var result = handler.Handle(command);

				stopwatch.Stop();
				onInvocationCompletedSuccessfully(command, result, stopwatch.Elapsed);
				return result;
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				onInvocationException.Invoke(command, ex, stopwatch.Elapsed);
				throw;
			}
		}
	}
}