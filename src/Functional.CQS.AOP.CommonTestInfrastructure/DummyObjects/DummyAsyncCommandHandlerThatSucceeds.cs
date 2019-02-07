using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IAsyncCommandHandler{TCommand, TError}"/> implementation.  Returns a success result.
	/// </summary>
	public class DummyAsyncCommandHandlerThatSucceeds : IAsyncCommandHandler<DummyAsyncCommandThatSucceeds, DummyAsyncCommandError>
	{
		private static readonly DummyAsyncCommandThatSucceeds _command = new DummyAsyncCommandThatSucceeds();
		private static readonly Result<Unit, DummyAsyncCommandError> _result = Result.Success<Unit, DummyAsyncCommandError>(Unit.Value);
		
		/// <summary>
		/// Handle the command asynchronously.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<Result<Unit, DummyAsyncCommandError>> HandleAsync(DummyAsyncCommandThatSucceeds command, CancellationToken cancellationToken)
		{
			return await Task.Run(() => _result, cancellationToken);
		}
	}
}