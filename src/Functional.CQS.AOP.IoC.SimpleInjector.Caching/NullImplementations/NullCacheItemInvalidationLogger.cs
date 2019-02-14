using System;
using Functional.CQS.AOP.Caching.Infrastructure;
using IQ.Vanilla.CQS.AOP.Caching.Invalidation;

namespace IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching.NullImplementations
{
	internal class NullCacheItemInvalidationLogger : ILogFunctionalCacheItemInvalidationOperations
	{
		public void LogCacheItemInvalidation(Type queryType, Type resultType, string cacheKey) { }

		public void LogCacheGroupInvalidation(string groupKey) { }

		public void LogCacheInvalidation() { }
	}
}