using System;
using AutoFixture;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.Caching.Infrastructure.MemoryCache;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class CacheCustomization : ICustomization
	{
		private readonly Action<IFunctionalCache> _setupAction;

		public CacheCustomization(Action<IFunctionalCache> setupAction)
		{
			_setupAction = setupAction ?? throw new ArgumentNullException(nameof(setupAction));
		}

		public void Customize(IFixture fixture)
		{
			var cache = new FunctionalMemoryCache();
			_setupAction.Invoke(cache);

			fixture.Inject<IFunctionalCache>(cache);
		}
	}
}