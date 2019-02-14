using System;
using AutoFixture;
using FakeItEasy;
using Functional.CQS.AOP.Caching.Infrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.Caching.Models;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests._Customizations
{
	internal class FakeCacheCustomization : ICustomization
	{
		private readonly bool _cacheOperationsSucceed;

		public FakeCacheCustomization(bool cacheOperationsSucceed)
		{
			_cacheOperationsSucceed = cacheOperationsSucceed;
		}

		public void Customize(IFixture fixture)
		{
			var cache = A.Fake<IFunctionalCache>();
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<int>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<string>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<DataWrapper<string>>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<DummyQueryReturnsValueTypeResult>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<DummyQueryReturnsReferenceTypeResult>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<DataWrapper<DummyQueryReturnsReferenceTypeResult>>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<DummyQueryReturnsNullResult>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Add(A<string>._, A<Option<string>>._, A<DataWrapper<DummyQueryReturnsNullResult>>._, A<TimeSpan>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Remove(A<string>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.RemoveGroup(A<string>._)).Returns(MakeResult(_cacheOperationsSucceed));
			A.CallTo(() => cache.Clear()).Returns(MakeResult(_cacheOperationsSucceed));
			fixture.Inject(cache);
		}

		private static Result<Unit, Exception> MakeResult(bool succeed)
		{
			return Result.Try(() => succeed ? Unit.Value : throw new Exception());
		}
	}
}