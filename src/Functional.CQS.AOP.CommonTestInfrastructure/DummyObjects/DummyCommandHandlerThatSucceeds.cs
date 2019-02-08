namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="ICommandHandler{TCommand, TError}"/> implementation.  Returns a success result.
	/// </summary>
	public class DummyCommandHandlerThatSucceeds : ICommandHandler<DummyCommandThatSucceeds, DummyCommandError>
	{
		/// <summary>
		/// Handle the command.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <returns></returns>
		public Result<Unit, DummyCommandError> Handle(DummyCommandThatSucceeds command)
		{
			return Result.Success<Unit, DummyCommandError>(Unit.Value);
		}
	}
}