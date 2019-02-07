using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation.  Returns null.
	/// </summary>
	public class DummyAsyncQueryReturnsNullHandler : IAsyncQueryHandler<DummyAsyncQueryReturnsNull, DummyAsyncQueryReturnsNullResult>, IProvideInformationAboutCQSHandlerDummyImplementation
	{
		private static readonly DummyAsyncQueryReturnsNull _query = new DummyAsyncQueryReturnsNull();
		private static readonly DummyAsyncQueryReturnsNullResult _result = null;

		/// <summary>
		/// Handle the query asynchronously.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<DummyAsyncQueryReturnsNullResult> HandleAsync(DummyAsyncQueryReturnsNull query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => _result, cancellationToken);
		}

		MethodInfo IProvideInformationAboutCQSHandlerDummyImplementation.GetHandleMethodInfo()
		{
			return GetType().GetMethod(nameof(HandleAsync));
		}

		object[] IProvideInformationAboutCQSHandlerDummyImplementation.GetArgumentsThatWillBePassedIntoDummyImplementationHandleMethod()
		{
			return new object[] { _query, new CancellationToken() };
		}

		object IProvideInformationAboutCQSHandlerDummyImplementation.GetValueThatWillBeReturnedFromDummyImplementationHandleMethod()
		{
			var asyncQueryWithReferenceTypeResultReturnValue = new Task<DummyAsyncQueryReturnsNullResult>(() => _result);
			asyncQueryWithReferenceTypeResultReturnValue.RunSynchronously();
			return asyncQueryWithReferenceTypeResultReturnValue;
		}
	}
}