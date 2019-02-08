using System;
using System.Collections.Generic;
using System.Linq;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;

namespace Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing
{
	internal static class ServiceAndImplementationTypeExtensions
	{
		public static bool HasMetricsCapturingStrategyDefined(this ServiceAndImplementationType context, IEnumerable<QueryAndResultType> queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection)
		{
			return context.ServiceType.GetGenericParametersForQueryHandlerType().Match(
				parameters => queryAndResultTypeWithMetricsCapturingStrategyDefinedCollection.Contains(parameters),
				() => false);
		}

		public static bool HasMetricsCapturingStrategyDefined(this ServiceAndImplementationType context, IEnumerable<CommandAndErrorType> commandAndErrorTypeWithMetricsCapturingStrategyDefinedCollection)
		{
			return context.ServiceType.GetGenericParametersForCommandHandlerType().Match(
				parameters => commandAndErrorTypeWithMetricsCapturingStrategyDefinedCollection.Contains(parameters),
				() => false);
		}
	}
}