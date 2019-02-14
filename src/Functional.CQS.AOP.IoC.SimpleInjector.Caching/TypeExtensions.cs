using System;
using System.Collections.Generic;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;

// ReSharper disable once CheckNamespace
namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching
{
	internal static class TypeExtensions
	{
		public static Option<QueryAndResultType> GetGenericParametersForQueryCachingStrategyType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceTypes(_queryCachingStrategyTypeCollection).Select(queryHandlerInterface =>
			{
				var queryType = queryHandlerInterface.GenericTypeArguments[0];
				var resultType = queryHandlerInterface.GenericTypeArguments[1];
				return new QueryAndResultType(queryType, resultType);
			});
		}

		private static readonly IEnumerable<Type> _queryCachingStrategyTypeCollection = new HashSet<Type>()
		{
			typeof(IQueryResultCachingStrategy<,>)
		};
	}
}