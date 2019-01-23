namespace Functional.CQS
{
	/// <summary>
	/// Interface for synchronous command handlers.
	/// </summary>
	/// <typeparam name="TCommand">The command type.</typeparam>
	/// <typeparam name="TError">The error type.</typeparam>
	public interface ICommandHandler<in TCommand, TError>
		where TCommand : ICommandParameters<TError>
	{
		/// <summary>
		/// Execute the command.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		/// <returns></returns>
		Result<Unit, TError> Handle(TCommand parameters);
	}
}