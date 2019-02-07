using System.Reflection;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="ICommandHandler{TCommand, TError}"/> implementation.  Returns a failure result.
	/// </summary>
	public class DummyCommandHandlerThatFails : ICommandHandler<DummyCommandThatFails, DummyCommandError>
	{
		private static readonly DummyCommandThatFails _command = new DummyCommandThatFails();

		/// <summary>
		/// Gets the return value for this handler.
		/// </summary>
		private static readonly Result<Unit, DummyCommandError> _result = Result.Failure<Unit, DummyCommandError>(new DummyCommandError());

		/// <summary>
		/// Handle the command.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <returns></returns>
		public Result<Unit, DummyCommandError> Handle(DummyCommandThatFails command)
		{
			return _result;
		}
	}
}