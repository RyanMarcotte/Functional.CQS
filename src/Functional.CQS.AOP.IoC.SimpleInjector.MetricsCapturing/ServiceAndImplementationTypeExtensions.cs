using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Installation;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Installation.Models;

namespace IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.MetricsCapturing
{
	internal static class ServiceAndImplementationTypeExtensions
	{
		public static bool HasMetricsCapturingStrategyDefined(this ServiceAndImplementationType context, IEnumerable<QueryAndResultType> queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection)
		{
			return context.ServiceType.GetGenericParametersForQueryHandlerType().Match(
				parameters => queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection.Contains(parameters),
				() => false);
		}

		public static bool HasMetricsCapturingStrategyDefined(this ServiceAndImplementationType context, IEnumerable<Type> commandTypeWithMetricsCapturingStrategyDefinedCollection)
		{
			return context.ServiceType.GetGenericParametersForCommandHandlerType().Match(
				parameters => commandTypeWithMetricsCapturingStrategyDefinedCollection.Contains(parameters),
				() => false);
		}

		public static bool HasMetricsCapturingStrategyDefined(this ServiceAndImplementationType context, IEnumerable<CommandAndErrorType> commandAndErrorTypeWithMetricsCapturingStrategyDefinedCollection)
		{
			return context.ServiceType.GetGenericParametersForResultCommandHandlerType().Match(
				parameters => commandAndErrorTypeWithMetricsCapturingStrategyDefinedCollection.Contains(parameters),
				() => false);
		}
	}
}