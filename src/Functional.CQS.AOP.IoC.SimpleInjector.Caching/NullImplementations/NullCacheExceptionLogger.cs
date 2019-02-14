using System;
using Functional.CQS.AOP.Caching.Infrastructure;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching.NullImplementations
{
	internal class NullCacheExceptionLogger : ILogFunctionalCacheExceptions
	{
		public void LogException(Type queryType, Type resultType, string cacheKey, Exception exception) { }

		public void LogException(Exception exception) { }
	}
}