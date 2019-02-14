using System;
using System.Diagnostics;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Extensions
{
	internal static class QueryHandlerExtensions
	{
		public static TResult HandleWithMetricsCapturing<TQuery, TResult>(
			this IQueryHandler<TQuery, TResult> handler,
			TQuery query,
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
				var result = handler.Handle(query);

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