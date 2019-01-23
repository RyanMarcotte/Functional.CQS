using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS
{
	/// <summary>
	/// Interface for asynchronous command handlers.
	/// </summary>
	/// <typeparam name="TCommand">The command type.</typeparam>
	/// <typeparam name="TError">The error type.</typeparam>
	public interface IAsyncCommandHandler<in TCommand, TError>
		where TCommand : ICommandParameters<TError>
	{
		/// <summary>
		/// Execute the command.
		/// </summary>
		/// <param name="parameters">The command parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		Task<Result<Unit, TError>> HandleAsync(TCommand parameters, CancellationToken cancellationToken);
	}
}