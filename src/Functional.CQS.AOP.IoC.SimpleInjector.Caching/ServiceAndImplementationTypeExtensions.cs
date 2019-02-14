using System.Collections.Generic;
using System.Linq;
using Functional.CQS.AOP.IoC.SimpleInjector;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;

namespace IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching
{
	internal static class ServiceAndImplementationTypeExtensions
	{
		public static bool ReturnsValueTypeAndHasCachingStrategyDefined(this ServiceAndImplementationType context, IEnumerable<QueryAndResultType> queryAndResultTypeWithCachingStrategyDefinedCollection)
		{
			return context.ServiceType.GetGenericParametersForQueryHandlerType().Match(
				parameters => parameters.ResultType.IsValueType && queryAndResultTypeWithCachingStrategyDefinedCollection.Contains(parameters),
				() => false);
		}

		public static bool ReturnsReferenceTypeAndHasCachingStrategyDefined(this ServiceAndImplementationType context, IEnumerable<QueryAndResultType> queryAndResultTypeWithCachingStrategyDefinedCollection)
		{
			return context.ServiceType.GetGenericParametersForQueryHandlerType().Match(
				parameters => !parameters.ResultType.IsValueType && queryAndResultTypeWithCachingStrategyDefinedCollection.Contains(parameters),
				() => false);
		}
	}
}