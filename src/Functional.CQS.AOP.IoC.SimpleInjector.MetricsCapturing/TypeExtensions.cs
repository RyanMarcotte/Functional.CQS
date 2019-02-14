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

		public static bool IsMetricsCapturingStrategyForQueryType(this Type type)
		{
			return type.IsClass && GetGenericParametersForQueryMetricsCapturingStrategyType(type).Match(
				queryAndResultType => typeof(IMetricsCapturingStrategyForQuery<,>).MakeGenericType(queryAndResultType.QueryType, queryAndResultType.ResultType).IsAssignableFrom(type),
				() => false);
		}

		public static bool IsMetricsCapturingStrategyForCommandType(this Type type)
		{
			return type.IsClass && GetGenericParametersForCommandMetricsCapturingStrategyType(type).Match(
				commandAndErrorType => typeof(IMetricsCapturingStrategyForCommand<,>).MakeGenericType(commandAndErrorType.CommandType, commandAndErrorType.ErrorType).IsAssignableFrom(type),
				() => false);
		}
	}
}