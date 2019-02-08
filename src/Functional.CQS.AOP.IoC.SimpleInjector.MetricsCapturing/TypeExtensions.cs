using System;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Installation;
using IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Installation.Models;
using IQ.Vanilla.CQS.AOP.MetricsCapturing;

namespace IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.MetricsCapturing
{
	internal static class TypeExtensions
	{
		public static Option<QueryAndResultType> GetGenericParametersForQueryMetricsCapturingStrategyType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceType(typeof(IMetricsCapturingStrategyForQuery<,>)).Map(queryHandlerInterface =>
			{
				var queryType = queryHandlerInterface.GenericTypeArguments[0];
				var resultType = queryHandlerInterface.GenericTypeArguments[1];
				return new QueryAndResultType(queryType, resultType);
			});
		}

		public static Option<Type> GetGenericParametersForCommandMetricsCapturingStrategyType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceType(typeof(IMetricsCapturingStrategyForCommand<>)).Map(commandHandlerInterface => commandHandlerInterface.GenericTypeArguments[0]);
		}

		public static Option<CommandAndErrorType> GetGenericParametersForResultCommandMetricsCapturingStrategyType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceType(typeof(IMetricsCapturingStrategyForResultCommand<,>)).Map(resultCommandHandlerInterface =>
			{
				var commandType = resultCommandHandlerInterface.GenericTypeArguments[0];
				var errorType = resultCommandHandlerInterface.GenericTypeArguments[1];
				return new CommandAndErrorType(commandType, errorType);
			});
		}
	}
}