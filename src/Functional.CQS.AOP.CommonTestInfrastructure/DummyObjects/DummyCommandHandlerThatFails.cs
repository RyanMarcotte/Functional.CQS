namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="ICommandHandler{TCommand, TError}"/> implementation.  Returns a failure result.
	/// </summary>
	public class DummyCommandHandlerThatFails : ICommandHandler<DummyCommandThatFails, DummyCommandError>
	{
		/// <summary>
		/// Handle the command.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <returns></returns>
		public Result<Unit, DummyCommandError> Handle(DummyCommandThatFails command)
		{
			return Result.Failure<Unit, DummyCommandError>(new DummyCommandError());
		}
	}
}