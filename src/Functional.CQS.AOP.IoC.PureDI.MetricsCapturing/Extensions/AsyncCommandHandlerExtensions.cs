using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Functional;
using Functional.CQS;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions
{
	internal static class AsyncCommandHandlerExtensions
	{
		public static async Task<Result<Unit, TError>> HandleAsyncWithMetricsCapturing<TCommand, TError>(
			this IAsyncCommandHandler<TCommand, TError> handler,
			TCommand command,
			CancellationToken cancellationToken,
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
				var result = await handler.HandleAsync(command, cancellationToken);

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