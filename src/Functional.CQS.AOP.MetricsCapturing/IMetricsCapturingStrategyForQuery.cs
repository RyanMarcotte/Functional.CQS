using System;

namespace Functional.CQS.AOP.MetricsCapturing
{
	/// <summary>
	/// Interface for metrics capturing strategies used by <see cref="IQueryHandler{TQuery, TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/>.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The type of data returned from the query.</typeparam>
	public interface IMetricsCapturingStrategyForQuery<in TQuery, in TResult>
		where TQuery : IQueryParameters<TResult>
	{
		/// <summary>
		/// Called immediately prior to handling the query.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		void OnInvocationStart(TQuery parameters);

		/// <summary>
		/// Called immediately after successfully handling the query and receiving a result.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="result">The query result.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		void OnInvocationCompletedSuccessfully(TQuery parameters, TResult result, TimeSpan timeElapsed);

		/// <summary>
		/// Called immediately after the query failed due to an uncaught exception.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="exception">The exception that caused the query handler to fail.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		void OnInvocationException(TQuery parameters, Exception exception, TimeSpan timeElapsed);
	}
}
