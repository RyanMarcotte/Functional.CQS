using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects;
using Functional.CQS.AOP.MetricsCapturing;
using Xunit;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests
{
	public class CommandHandlerMetricsCapturingDecoratorForUniversalStrategyTests
	{
		[Theory]
		[CommandHandlerCompletesSuccessfully]
		public void ShouldCaptureElapsedTime(
			CommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyCommandThatSucceeds, DummyCommandError> sut,
			IUniversalMetricsCapturingStrategy metricsCapturingStrategy)
		{
			var command = new DummyCommandThatSucceeds();
			sut.Handle(command);

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart()).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(A<TimeSpan>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(A<Exception>._, A<TimeSpan>._)).MustNotHaveHappened();
		}

		[Theory]
		[CommandHandlerThrowsException]
		public void ShouldCaptureExceptionAndElapsedTime(
			CommandHandlerMetricsCapturingDecoratorForUniversalStrategy<DummyCommandThatSucceeds, DummyCommandError> sut,
			IUniversalMetricsCapturingStrategy metricsCapturingStrategy)
		{
			var command = new DummyCommandThatSucceeds();
			Result.Try(() => sut.Handle(command));

			A.CallTo(() => metricsCapturingStrategy.OnInvocationStart()).MustHaveHappenedOnceExactly();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationCompletedSuccessfully(A<TimeSpan>._)).MustNotHaveHappened();
			A.CallTo(() => metricsCapturingStrategy.OnInvocationException(A<Exception>._, A<TimeSpan>._)).MustHaveHappenedOnceExactly();
		}

		#region Arrangements

		private abstract class CommandHandlerMetricsCapturingDecoratorTestsArrangementBase : AutoDataAttribute
		{
			protected CommandHandlerMetricsCapturingDecoratorTestsArrangementBase(Func<Result<Unit, DummyCommandError>> resultFactory)
				: base(() => new Fixture()
					.Customize(new CommandHandlerCustomization(resultFactory))
					.Customize(new MetricsCapturingStrategyCustomization()))
			{

			}
		}

		private class CommandHandlerCompletesSuccessfully : CommandHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public CommandHandlerCompletesSuccessfully()
				: base(() => Result.Success<Unit, DummyCommandError>(Unit.Value))
			{
			}
		}

		private class CommandHandlerThrowsException : CommandHandlerMetricsCapturingDecoratorTestsArrangementBase
		{
			public CommandHandlerThrowsException()
				: base(() => throw new Exception())
			{
			}
		}

		#endregion

		#region Customizations

		private class CommandHandlerCustomization : ICustomization
		{
			private readonly Func<Result<Unit, DummyCommandError>> _resultFactory;

			public CommandHandlerCustomization(Func<Result<Unit, DummyCommandError>> resultFactory)
			{
				_resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
			}

			public void Customize(IFixture fixture)
			{
				var commandHandler = A.Fake<ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>>();
				A.CallTo(() => commandHandler.Handle(A<DummyCommandThatSucceeds>._)).ReturnsLazily(_resultFactory);

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