using System;
using IQ.Vanilla.CQS.AOP.Caching.Invalidation;

namespace IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching.NullImplementations
{
	internal class NullCacheItemReplacementLogger : ILogFunctionalCacheItemReplacementOperations
	{
		public void LogCacheItemReplacement(Type queryType, Type resultType, string cacheKey) { }
	}
}