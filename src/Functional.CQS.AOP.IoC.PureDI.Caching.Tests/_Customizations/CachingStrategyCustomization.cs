using AutoFixture;
using Functional.CQS.AOP.Caching;
using Functional.CQS.AOP.CommonTestInfrastructure.Caching;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class CachingStrategyCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Inject<IQueryResultCachingStrategy<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>>(new DummyQueryReturnsValueTypeCachingStrategy());
			fixture.Inject<IQueryResultCachingStrategy<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>>(new DummyQueryReturnsReferenceTypeCachingStrategy());
			fixture.Inject<IQueryResultCachingStrategy<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>>(new DummyAsyncQueryReturnsValueTypeCachingStrategy());
			fixture.Inject<IQueryResultCachingStrategy<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>>(new DummyAsyncQueryReturnsReferenceTypeCachingStrategy());
		}
	}
}