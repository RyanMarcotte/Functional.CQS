using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests._Customizations;
using Functional.CQS.AOP.MetricsCapturing;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests
{
	public class AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategyTests
	{
		[Theory]
		[AsyncCommandHandlerCompletesSuccessfully]
		public async Task ShouldCaptureResultAndElapsedTime(
			AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError> sut,
			IUniversalMetricsCapturingStrategy metricsCapturingStrategy)
		{
			var command = new DummyAsyncCommandThatSucceeds();
			await sut.HandleAsync(command, new CancellationToken());

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart()).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(A<TimeSpan>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(A<Exception>._, A<TimeSpan>._)).MustNotHaveHappened();
		}

		[Theory]
		[AsyncCommandHandlerThrowsException]
		public async Task ShouldCaptureExceptionAndElapsedTime(
			AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError> sut,
			IUniversalMetricsCapturingStrategy metricsCapturingStrategy)
		{
			var command = new DummyAsyncCommandThatSucceeds();
			await Result.Try(async () => await sut.HandleAsync(command, new CancellationToken()));

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart()).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(A<TimeSpan>._)).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(A<Exception>._, A<TimeSpan>._)).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[AsyncCommandHandlerCompletesSuccessfullyAndDecoratorIsDisabled]
		[AsyncCommandHandlerThrowsExceptionAndDecoratorIsDisabled]
		public async Task DoesNotExecuteAnyDecorationCodeIfDecoratorIsDisabled(
			AsyncCommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError> sut,
			IUniversalMetricsCapturingStrategy metricsCapturingStrategy)
		{
			var command = new DummyAsyncCommandThatSucceeds();
			await Result.Try(async () => await sut.HandleAsync(command, new CancellationToken()));

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart()).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(A<TimeSpan>._)).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(A<Exception>._, A<TimeSpan>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class AsyncCommandHandlerMetricsCapturingDecoratorTestsArrangementBase : AutoDataAttribute
		{
			protected AsyncCommandHandlerMetricsCapturingDecoratorTestsArrangementBase(Func<Task<Result<Unit, DummyAsyncCommandError>>> resultFactory, bool decoratorEnabled)
				: base(() => new Fixture()
					.Customize(new AsyncCommandHandlerCustomization(resultFactory))
					.Customize(new MetricsCapturingStrategyCustomization())
					.Customize(new MetricsCapturingModuleConfigurationParametersCustomization(new MetricsCapturingModuleConfigurationParameters(decoratorEnabled, decoratorEnabled, decoratorEnabled))))
			{

			}
		}

		private class AsyncCommandHandlerCompletesSuccessfully : AsyncCommandHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public AsyncCommandHandlerCompletesSuccessfully()
				: base(() => Task.FromResult(Result.Success<Unit, DummyAsyncCommandError>(Unit.Value)), true)
			{
			}
		}

		private class AsyncCommandHandlerThrowsException : AsyncCommandHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public AsyncCommandHandlerThrowsException()
				: base(() => Task.FromException<Result<Unit, DummyAsyncCommandError>>(new Exception()), true)
			{
			}
		}

		private class AsyncCommandHandlerCompletesSuccessfullyAndDecoratorIsDisabled : AsyncCommandHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public AsyncCommandHandlerCompletesSuccessfullyAndDecoratorIsDisabled()
				: base(() => Task.FromResult(Result.Success<Unit, DummyAsyncCommandError>(Unit.Value)), false)
			{
			}
		}

		private class AsyncCommandHandlerThrowsExceptionAndDecoratorIsDisabled : AsyncCommandHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public AsyncCommandHandlerThrowsExceptionAndDecoratorIsDisabled()
				: base(() => Task.FromException<Result<Unit, DummyAsyncCommandError>>(new Exception()), false)
			{
			}
		}

		#endregion

		#region Customizations

		private class AsyncCommandHandlerCustomization : ICustomization
		{
			private readonly Func<Task<Result<Unit, DummyAsyncCommandError>>> _resultFactory;

			public AsyncCommandHandlerCustomization(Func<Task<Result<Unit, DummyAsyncCommandError>>> resultFactory)
			{
				_resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
			}

			public void Customize(IFixture fixture)
			{
				var commandHandler = A.Fake<IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>>();
				A.CallTo(() => commandHandler.HandleAsync(A<DummyAsyncCommandThatSucceeds>._, A<CancellationToken>._)).ReturnsLazily(_resultFactory);

				fixture.Inject(commandHandler);
			}
		}

		private class MetricsCapturingStrategyCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Inject(A.Fake<IUniversalMetricsCapturingStrategy>());
			}
		}

		#endregion
	}
}