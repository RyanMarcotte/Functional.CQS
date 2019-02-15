using System;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.CommonTestInfrastructure.MetricsCapturing.DummyObjects
{
	/// <summary>
	/// Metrics capturing strategy for <see cref="DummyQueryReturnsReferenceType"/>.
	/// </summary>
	public class DummyQueryReturnsReferenceTypeMetricsCapturingStrategy : IMetricsCapturingStrategyForQuery<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>
	{
		/// <summary>
		/// Called immediately prior to handling the query.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		public void OnInvocationStart(DummyQueryReturnsReferenceType parameters)
		{
			
		}

		/// <summary>
		/// Called immediately after successfully handling the query and receiving a result.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="result">The query result.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		public void OnInvocationCompletedSuccessfully(DummyQueryReturnsReferenceType parameters, DummyQueryReturnsReferenceTypeResult result, TimeSpan timeElapsed)
		{
			
		}

		/// <summary>
		/// Called immediately after the query failed.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="exception">The exception that caused the query handler to fail.</param>
		/// <param name="timeElapsed">The time elapsed since the start of invocation.</param>
		public void OnInvocationException(DummyQueryReturnsReferenceType parameters, Exception exception, TimeSpan timeElapsed)
		{

		}
	}
}