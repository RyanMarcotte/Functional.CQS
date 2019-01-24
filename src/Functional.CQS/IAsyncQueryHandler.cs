using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS
{
	/// <summary>
	/// Interface for asynchronous query handlers.  Intended to encapsulate application logic that returns a result and is free of side effects (i.e. no observable changes made to system).
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public interface IAsyncQueryHandler<in TQuery, TResult>
		where TQuery : IQueryParameters<TResult>
	{
		/// <summary>
		/// Execute the query.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		Task<TResult> HandleAsync(TQuery parameters, CancellationToken cancellationToken);
	}
}