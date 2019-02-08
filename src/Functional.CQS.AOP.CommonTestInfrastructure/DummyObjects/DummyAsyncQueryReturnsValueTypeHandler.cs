using System.Threading;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation.  Returns value type.
	/// </summary>
	public class DummyAsyncQueryReturnsValueTypeHandler : IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>
	{
		/// <summary>
		/// Handle the query asynchronously.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<DummyAsyncQueryReturnsValueTypeResult> HandleAsync(DummyAsyncQueryReturnsValueType query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => new DummyAsyncQueryReturnsValueTypeResult(), cancellationToken);
		}
	}
}