using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation.  Returns null.
	/// </summary>
	public class DummyAsyncQueryReturnsNullHandler : IAsyncQueryHandler<DummyAsyncQueryReturnsNull, DummyAsyncQueryReturnsNullResult>
	{
		/// <summary>
		/// Handle the query asynchronously.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<DummyAsyncQueryReturnsNullResult> HandleAsync(DummyAsyncQueryReturnsNull query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => (DummyAsyncQueryReturnsNullResult)null, cancellationToken);
		}
	}
}