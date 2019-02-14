using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests
{
	public class QueryHandlerMetricsCapturingDecoratorTests
	{
		[Theory]
		[QueryHandlerCompletesSuccessfully]
		public void ShouldCaptureResultAndElapsedTime(
			QueryHandlerMetricsCapturingDecorator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IMetricsCapturingStrategyForQuery<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> metricsCapturingStrategy)
		{
			var query = new DummyQueryReturnsValueType();
			sut.Handle(query);

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart(query)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(query, A<DummyQueryReturnsValueTypeResult>._, A<TimeSpan>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(query, A<Exception>._, A<TimeSpan>._)).MustNotHaveHappened();
		}

		[Theory]
		[QueryHandlerThrowsException]
		public void ShouldCaptureExceptionAndElapsedTime(
			QueryHandlerMetricsCapturingDecorator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IMetricsCapturingStrategyForQuery<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> metricsCapturingStrategy)
		{
			var query = new DummyQueryReturnsValueType();
			Result.Try(() => sut.Handle(query));
			
			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart(query)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(query, A<DummyQueryReturnsValueTypeResult>._, A<TimeSpan>._)).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(query, A<Exception>._, A<TimeSpan>._)).MustHaveHappenedOnceExactly();
		}

		#region Arrangements

		private abstract class QueryHandlerMetricsCapturingDecoratorTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerMetricsCapturingDecoratorTestsArrangementBase(Func<DummyQueryReturnsValueTypeResult> resultFactory)
			: base(() => new Fixture()
				.Customize(new QueryHandlerCustomization(resultFactory))
				.Customize(new MetricsCapturingStrategyCustomization()))
			{

			}
		}

		private class QueryHandlerCompletesSuccessfully : QueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public QueryHandlerCompletesSuccessfully()
				: base(() => new DummyQueryReturnsValueTypeResult())
			{
			}
		}

		private class QueryHandlerThrowsException : QueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public QueryHandlerThrowsException()
				: base(() => throw new Exception())
			{
			}
		}

		#endregion

		#region Customizations

		private class QueryHandlerCustomization : ICustomization
		{
			private readonly Func<DummyQueryReturnsValueTypeResult> _resultFactory;

			public QueryHandlerCustomization(Func<DummyQueryReturnsValueTypeResult> resultFactory)
			{
				_resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
			}

			public void Customize(IFixture fixture)
			{
				var queryHandler = A.Fake<IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>>();
				A.CallTo(() => queryHandler.Handle(A<DummyQueryReturnsValueType>._)).ReturnsLazily(_resultFactory);

				fixture.Inject(queryHandler);
			}
		}

		private class MetricsCapturingStrategyCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Inject(A.Fake<IMetricsCapturingStrategyForQuery<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>>());
			}
		}

		#endregion
	}
}
