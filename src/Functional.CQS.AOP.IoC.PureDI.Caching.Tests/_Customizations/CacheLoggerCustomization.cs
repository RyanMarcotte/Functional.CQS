using AutoFixture;
using FakeItEasy;
using Functional.CQS.AOP.Caching.Infrastructure;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class CacheLoggerCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Inject(A.Fake<ILogFunctionalCacheHitsAndMisses>());
			fixture.Inject(A.Fake<ILogFunctionalCacheExceptions>());
		}
	}
}