using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;
using Functional.Primitives.FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Tests
{
	public class TypeExtensionsTests
	{
		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyQueryReturnsValueTypeHandler()
			=> VerifyQueryHandlerType<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult, DummyQueryReturnsValueTypeHandler>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyQueryReturnsReferenceTypeHandler()
			=> VerifyQueryHandlerType<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult, DummyQueryReturnsReferenceTypeHandler>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyAsyncQueryReturnsValueTypeHandler()
			=> VerifyAsyncQueryHandlerType<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult, DummyAsyncQueryReturnsValueTypeHandler>();

		[Fact]
		public void ShouldReturnExpectedQueryAndResultTypeForDummyAsyncQueryReturnsReferenceTypeHandler()
			=> VerifyAsyncQueryHandlerType<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult, DummyAsyncQueryReturnsReferenceTypeHandler>();

		private static void VerifyQueryHandlerType<TQuery, TResult, TQueryHandler>()
			where TQuery : IQueryParameters<TResult>
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			typeof(TQueryHandler).GetGenericParametersForQueryHandlerType()
				.Should()
				.HaveValue()
				.AndValue
				.Should()
				.Match<QueryAndResultType>(x => x.QueryType == typeof(TQuery) && x.ResultType == typeof(TResult));
		}

		private static void VerifyAsyncQueryHandlerType<TQuery, TResult, TQueryHandler>()
			where TQuery : IQueryParameters<TResult>
			where TQueryHandler : IAsyncQueryHandler<TQuery, TResult>
		{
			typeof(TQueryHandler).GetGenericParametersForQueryHandlerType()
				.Should()
				.HaveValue()
				.AndValue
				.Should()
				.Match<QueryAndResultType>(x => x.QueryType == typeof(TQuery) && x.ResultType == typeof(TResult));
		}
	}
}