using System.Reflection;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="ICommandHandler{TCommand, TError}"/> implementation.  Returns a success result.
	/// </summary>
	public class DummyCommandHandlerThatSucceeds : ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>, IProvideInformationAboutCQSHandlerDummyImplementation
	{
		private static readonly DummyCommandThatSucceeds _command = new DummyCommandThatSucceeds();

		/// <summary>
		/// Gets the return value for this handler.
		/// </summary>
		private static readonly Result<Unit, DummyCommandError> _result = Result.Success<Unit, DummyCommandError>(Unit.Value);

		/// <summary>
		/// Handle the command.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <returns></returns>
		public Result<Unit, DummyCommandError> Handle(DummyCommandThatSucceeds command)
		{
			return _result;
		}

		MethodInfo IProvideInformationAboutCQSHandlerDummyImplementation.GetHandleMethodInfo()
		{
			return GetType().GetMethod(nameof(Handle));
		}

		object[] IProvideInformationAboutCQSHandlerDummyImplementation.GetArgumentsThatWillBePassedIntoDummyImplementationHandleMethod()
		{
			return new object[] { _command };
		}

		object IProvideInformationAboutCQSHandlerDummyImplementation.GetValueThatWillBeReturnedFromDummyImplementationHandleMethod()
		{
			return _result;
		}
	}
}