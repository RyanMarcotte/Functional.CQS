using System.Reflection;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Sample <see cref="IQueryHandler{TQuery, TResult}"/> implementation.  Returns value type.
	/// </summary>
	public class DummyQueryReturnsValueTypeHandler : IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>, IProvideInformationAboutCQSHandlerDummyImplementation
	{
		private static readonly DummyQueryReturnsValueType _query = new DummyQueryReturnsValueType();
		private static readonly DummyQueryReturnsValueTypeResult _result = new DummyQueryReturnsValueTypeResult();

		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		public DummyQueryReturnsValueTypeResult Handle(DummyQueryReturnsValueType query)
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