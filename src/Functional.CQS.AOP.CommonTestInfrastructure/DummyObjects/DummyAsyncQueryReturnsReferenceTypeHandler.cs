using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Sample <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation.  Returns reference type.
	/// </summary>
	public class DummyAsyncQueryReturnsReferenceTypeHandler : IAsyncQueryHandler<DummyAsyncQueryReturnsReferenceType, DummyAsyncQueryReturnsReferenceTypeResult>, IProvideInformationAboutCQSHandlerDummyImplementation
	{
		private static readonly DummyAsyncQueryReturnsReferenceType _query = new DummyAsyncQueryReturnsReferenceType();
		private static readonly DummyAsyncQueryReturnsReferenceTypeResult _result = new DummyAsyncQueryReturnsReferenceTypeResult();

		/// <summary>
		/// Handle the query asynchronously.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<DummyAsyncQueryReturnsReferenceTypeResult> HandleAsync(DummyAsyncQueryReturnsReferenceType query, CancellationToken cancellationToken = new CancellationToken())
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
			var asyncQueryWithReferenceTypeResultReturnValue = new Task<DummyAsyncQueryReturnsReferenceTypeResult>(() => _result);
			asyncQueryWithReferenceTypeResultReturnValue.RunSynchronously();
			return asyncQueryWithReferenceTypeResultReturnValue;
		}
	}
}