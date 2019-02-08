using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IAsyncCommandHandler{TCommand, TError}"/> implementation.  Returns a failure result.
	/// </summary>
	public class DummyAsyncCommandHandlerThatFails : IAsyncCommandHandler<DummyAsyncCommandThatFails, DummyAsyncCommandError>
	{
		/// <summary>
		/// Handles the command asynchronously.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<Result<Unit, DummyAsyncCommandError>> HandleAsync(DummyAsyncCommandThatFails command, CancellationToken cancellationToken)
		{
			return await Task.Run(() => Result.Failure<Unit, DummyAsyncCommandError>(new DummyAsyncCommandError()), cancellationToken);
		}
	}
}