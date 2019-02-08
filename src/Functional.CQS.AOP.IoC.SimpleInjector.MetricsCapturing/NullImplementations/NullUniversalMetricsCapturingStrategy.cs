using System;
using IQ.Vanilla.CQS.AOP.MetricsCapturing;

namespace IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.NullImplementations
{
	internal class NullUniversalMetricsCapturingStrategy : IUniversalMetricsCapturingStrategy
	{
		public void OnInvocationStart() { }
		public void OnInvocationCompletedSuccessfully(TimeSpan timeElapsed) { }
		public void OnInvocationException(Exception exception, TimeSpan timeElapsed) { }
	}
}