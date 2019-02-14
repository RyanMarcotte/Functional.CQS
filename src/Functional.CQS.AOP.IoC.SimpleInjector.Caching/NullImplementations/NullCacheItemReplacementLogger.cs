using System;
using Functional.CQS.AOP.Caching.Infrastructure;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching.NullImplementations
{
	internal class NullCacheItemReplacementLogger : ILogFunctionalCacheItemReplacementOperations
	{
		public void LogCacheItemReplacement(Type queryType, Type resultType, string cacheKey) { }
	}
}