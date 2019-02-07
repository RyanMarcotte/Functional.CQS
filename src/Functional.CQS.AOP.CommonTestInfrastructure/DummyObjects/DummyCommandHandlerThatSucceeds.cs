using System.Reflection;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="ICommandHandler{TCommand, TError}"/> implementation.  Returns a success result.
	/// </summary>
	public class DummyCommandHandlerThatSucceeds : ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>
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
	}
}