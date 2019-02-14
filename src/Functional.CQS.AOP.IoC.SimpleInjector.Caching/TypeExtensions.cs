using System;
using System.Collections.Generic;
using Functional;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.IoC.SimpleInjector;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;

// ReSharper disable once CheckNamespace
namespace SimpleInjector
{
	internal static class TypeExtensions
	{
		public static Option<QueryAndResultType> GetGenericParametersForQueryCachingStrategyType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceTypes(_queryCachingStrategyTypeCollection).Map(queryHandlerInterface =>
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