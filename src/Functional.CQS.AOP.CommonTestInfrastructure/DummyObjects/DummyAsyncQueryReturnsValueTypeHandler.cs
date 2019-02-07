using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Sample <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation.  Returns value type.
	/// </summary>
	public class DummyAsyncQueryReturnsValueTypeHandler : IAsyncQueryHandler<DummyAsyncQueryReturnsValueType, DummyAsyncQueryReturnsValueTypeResult>, IProvideInformationAboutCQSHandlerDummyImplementation
	{
		private static readonly DummyAsyncQueryReturnsValueType _query = new DummyAsyncQueryReturnsValueType();
		private static readonly DummyAsyncQueryReturnsValueTypeResult _result = new DummyAsyncQueryReturnsValueTypeResult();

		/// <summary>
		/// Handle the query asynchronously.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<DummyAsyncQueryReturnsValueTypeResult> HandleAsync(DummyAsyncQueryReturnsValueType query, CancellationToken cancellationToken = new CancellationToken())
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
			var asyncQueryWithValueTypeResultReturnValue = new Task<DummyAsyncQueryReturnsValueTypeResult>(() => _result);
			asyncQueryWithValueTypeResultReturnValue.RunSynchronously();
			return asyncQueryWithValueTypeResultReturnValue;
		}
	}
}