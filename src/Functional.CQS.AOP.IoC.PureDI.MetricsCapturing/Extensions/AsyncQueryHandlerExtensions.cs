using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions
{
	internal static class AsyncQueryHandlerExtensions
	{
		public static async Task<TResult> HandleAsyncWithMetricsCapturing<TQuery, TResult>(
			this IAsyncQueryHandler<TQuery, TResult> handler,
			TQuery query,
			CancellationToken cancellationToken,
			Action<TQuery> onInvocationStart,
			Action<TQuery, TResult, TimeSpan> onInvocationCompletedSuccessfully,
			Action<TQuery, Exception, TimeSpan> onInvocationException)
			where TQuery : IQueryParameters<TResult>
		{
			onInvocationStart.Invoke(query);

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			try
			{
				var result = await handler.HandleAsync(query, cancellationToken);

				stopwatch.Stop();
				onInvocationCompletedSuccessfully.Invoke(query, result, stopwatch.Elapsed);
				return result;
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				onInvocationException.Invoke(query, ex, stopwatch.Elapsed);
				throw;
			}
		}
	}
}