using System.Reflection;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Sample <see cref="IQueryHandler{TQuery, TResult}"/> implementation.  Returns reference type.
	/// </summary>
	public class DummyQueryReturnsReferenceTypeHandler : IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>, IProvideInformationAboutCQSHandlerDummyImplementation
	{
		private static readonly DummyQueryReturnsReferenceType _query = new DummyQueryReturnsReferenceType();
		private static readonly DummyQueryReturnsReferenceTypeResult _result = new DummyQueryReturnsReferenceTypeResult();

		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		public DummyQueryReturnsReferenceTypeResult Handle(DummyQueryReturnsReferenceType query)
		{
			return _result;
		}

		MethodInfo IProvideInformationAboutCQSHandlerDummyImplementation.GetHandleMethodInfo()
		{
			return GetType().GetMethod(nameof(Handle));
		}

		object[] IProvideInformationAboutCQSHandlerDummyImplementation.GetArgumentsThatWillBePassedIntoDummyImplementationHandleMethod()
		{
			return new object[] { _query };
		}

		object IProvideInformationAboutCQSHandlerDummyImplementation.GetValueThatWillBeReturnedFromDummyImplementationHandleMethod()
		{
			return _result;
		}
	}
}