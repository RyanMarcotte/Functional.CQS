using AutoFixture;
using FakeItEasy;
using IQ.Vanilla.CQS.AOP.Caching.Infrastructure;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
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