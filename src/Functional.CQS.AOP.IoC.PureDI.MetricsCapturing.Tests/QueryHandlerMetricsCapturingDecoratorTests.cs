using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional;
using Functional.CQS;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;
using IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests._Customizations;
using Xunit;

namespace IQ.Vanilla.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests
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

		[Theory]
		[QueryHandlerCompletesSuccessfullyAndDecoratorIsDisabled]
		[QueryHandlerThrowsExceptionAndDecoratorIsDisabled]
		public void DoesNotExecuteAnyDecorationCodeIfDecoratorIsDisabled(
			QueryHandlerMetricsCapturingDecorator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IMetricsCapturingStrategyForQuery<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> metricsCapturingStrategy)
		{
			var query = new DummyQueryReturnsValueType();
			Result.Try(() => sut.Handle(query));

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart(query)).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(query, A<DummyQueryReturnsValueTypeResult>._, A<TimeSpan>._)).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(query, A<Exception>._, A<TimeSpan>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class QueryHandlerMetricsCapturingDecoratorTestsArrangementBase : AutoDataAttribute
		{
			protected QueryHandlerMetricsCapturingDecoratorTestsArrangementBase(Func<DummyQueryReturnsValueTypeResult> resultFactory, bool decoratorEnabled)
			: base(() => new Fixture()
				.Customize(new QueryHandlerCustomization(resultFactory))
				.Customize(new MetricsCapturingStrategyCustomization())
				.Customize(new MetricsCapturingModuleConfigurationParametersCustomization(new MetricsCapturingModuleConfigurationParameters(decoratorEnabled, decoratorEnabled, decoratorEnabled))))
			{

			}
		}

		private class QueryHandlerCompletesSuccessfully : QueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public QueryHandlerCompletesSuccessfully()
				: base(() => new DummyQueryReturnsValueTypeResult(), true)
			{
			}
		}

		private class QueryHandlerThrowsException : QueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public QueryHandlerThrowsException()
				: base(() => throw new Exception(), true)
			{
			}
		}

		private class QueryHandlerCompletesSuccessfullyAndDecoratorIsDisabled : QueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public QueryHandlerCompletesSuccessfullyAndDecoratorIsDisabled()
				: base(() => new DummyQueryReturnsValueTypeResult(), false)
			{
			}
		}

		private class QueryHandlerThrowsExceptionAndDecoratorIsDisabled : QueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public QueryHandlerThrowsExceptionAndDecoratorIsDisabled()
				: base(() => throw new Exception(), false)
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
