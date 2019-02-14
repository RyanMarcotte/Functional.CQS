using System;
using Functional.CQS.AOP.Caching.Infrastructure;

namespace IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching.NullImplementations
{
	internal class NullCacheHitAndMissLogger : ILogFunctionalCacheHitsAndMisses
	{
		public void LogCacheHit(Type queryType, Type resultType, string cacheKey) { }

		public void LogCacheMiss(Type queryType, Type resultType, string cacheKey) { }
	}
}