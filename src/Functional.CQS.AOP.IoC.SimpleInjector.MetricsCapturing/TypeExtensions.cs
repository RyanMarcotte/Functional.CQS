using System;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;
using Functional.CQS.AOP.MetricsCapturing;

namespace Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing
{
	internal static class TypeExtensions
	{
		public static Option<QueryAndResultType> GetGenericParametersForQueryMetricsCapturingStrategyType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceType(typeof(IMetricsCapturingStrategyForQuery<,>)).Select(queryHandlerInterface =>
			{
				var queryType = queryHandlerInterface.GenericTypeArguments[0];
				var resultType = queryHandlerInterface.GenericTypeArguments[1];
				return new QueryAndResultType(queryType, resultType);
			});
		}

		public static Option<CommandAndErrorType> GetGenericParametersForCommandMetricsCapturingStrategyType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceType(typeof(IMetricsCapturingStrategyForCommand<,>)).Select(commandHandlerInterface =>
			{
				var commandType = commandHandlerInterface.GenericTypeArguments[0];
				var errorType = commandHandlerInterface.GenericTypeArguments[1];
				return new CommandAndErrorType(commandType, errorType);
			});
		}
	}
}