using System;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.CommonTestInfrastructure.MetricsCapturing.DummyObjects
{
	/// <summary>
	/// Metrics capturing strategy for <see cref="DummyAsyncQueryReturnsValueTypeHandler"/>.
	/// </summary>
	public class DummyAsyncQueryReturnsValueTypeMetricsCapturingStrategy : IMetricsCapturingStrategyForQuery<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>
	{
		/// <summary>
		/// Called immediately prior to handling the query.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		public void OnInvocationStart(DummyAsyncQueryReturnsValueType parameters)
		{

		}

		/// <summary>
		/// Called immediately after successfully handling the query and receiving a result.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="result">The query result.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		public void OnInvocationCompletedSuccessfully(DummyAsyncQueryReturnsValueType parameters, DummyAsyncQueryReturnsValueTypeResult result, TimeSpan timeElapsed)
		{

		}

		/// <summary>
		/// Called immediately after the query failed.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="exception">The exception that caused the query handler to fail.</param>
		/// <param name="timeElapsed"></param>
		public void OnInvocationException(DummyAsyncQueryReturnsValueType parameters, Exception exception, TimeSpan timeElapsed)
		{

		}
	}
}