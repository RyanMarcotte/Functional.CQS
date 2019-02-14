using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests
{
	public class AsyncQueryHandlerMetricsCapturingDecoratorTests
	{
		[Theory]
		[AsyncQueryHandlerCompletesSuccessfully]
		public async Task ShouldCaptureResultAndElapsedTime(
			AsyncQueryHandlerMetricsCapturingDecorator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IMetricsCapturingStrategyForQuery<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> metricsCapturingStrategy)
		{
			var query = new DummyQueryReturnsValueType();
			await sut.HandleAsync(query);

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart(query)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(query, A<DummyQueryReturnsValueTypeResult>._, A<TimeSpan>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(query, A<Exception>._, A<TimeSpan>._)).MustNotHaveHappened();
		}

		[Theory]
		[AsyncQueryHandlerThrowsException]
		public async Task ShouldCaptureExceptionAndElapsedTime(
			AsyncQueryHandlerMetricsCapturingDecorator<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> sut,
			IMetricsCapturingStrategyForQuery<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult> metricsCapturingStrategy)
		{
			var query = new DummyQueryReturnsValueType();
			await Result.Try(async () => await sut.HandleAsync(query));

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart(query)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(query, A<DummyQueryReturnsValueTypeResult>._, A<TimeSpan>._)).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(query, A<Exception>._, A<TimeSpan>._)).MustHaveHappenedOnceExactly();
		}

		#region Arrangements

		private abstract class AsyncQueryHandlerMetricsCapturingDecoratorTestsArrangementBase : AutoDataAttribute
		{
			protected AsyncQueryHandlerMetricsCapturingDecoratorTestsArrangementBase(Func<Task<DummyQueryReturnsValueTypeResult>> resultFactory)
				: base(() => new Fixture()
					.Customize(new AsyncQueryHandlerCustomization(resultFactory))
					.Customize(new MetricsCapturingStrategyCustomization()))
			{

			}
		}

		private class AsyncQueryHandlerCompletesSuccessfully : AsyncQueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public AsyncQueryHandlerCompletesSuccessfully()
				: base(() => Task.FromResult(new DummyQueryReturnsValueTypeResult()))
			{
			}
		}

		private class AsyncQueryHandlerThrowsException : AsyncQueryHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public AsyncQueryHandlerThrowsException()
				: base(() => Task.FromException<DummyQueryReturnsValueTypeResult>(new Exception()))
			{
			}
		}

		#endregion

		#region Customizations

		private class AsyncQueryHandlerCustomization : ICustomization
		{
			private readonly Func<Task<DummyQueryReturnsValueTypeResult>> _resultFactory;

			public AsyncQueryHandlerCustomization(Func<Task<DummyQueryReturnsValueTypeResult>> resultFactory)
			{
				_resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
			}

			public void Customize(IFixture fixture)
			{
				var queryHandler = A.Fake<IAsyncQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>>();
				A.CallTo(() => queryHandler.HandleAsync(A<DummyQueryReturnsValueType>._, A<CancellationToken>._)).ReturnsLazily(_resultFactory);

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