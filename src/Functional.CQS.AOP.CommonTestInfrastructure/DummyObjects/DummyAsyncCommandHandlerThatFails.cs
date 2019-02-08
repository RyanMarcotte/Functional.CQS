using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IAsyncCommandHandler{TCommand, TError}"/> implementation.  Returns a failure result.
	/// </summary>
	public class DummyAsyncCommandHandlerThatFails : IAsyncCommandHandler<DummyAsyncCommandThatFails, DummyAsyncCommandError>
	{
		private static readonly DummyAsyncCommandThatFails _command = new DummyAsyncCommandThatFails();
		private static readonly Result<Unit, DummyAsyncCommandError> _result = Result.Failure<Unit, DummyAsyncCommandError>(new DummyAsyncCommandError());

		/// <summary>
		/// Handles the command asynchronously.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<Result<Unit, DummyAsyncCommandError>> HandleAsync(DummyAsyncCommandThatFails command, CancellationToken cancellationToken)
		{
			return await Task.Run(() => _result, cancellationToken);
		}
	}
}