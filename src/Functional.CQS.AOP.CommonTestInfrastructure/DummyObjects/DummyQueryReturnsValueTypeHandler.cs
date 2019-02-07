using System.Reflection;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Sample <see cref="IQueryHandler{TQuery, TResult}"/> implementation.  Returns value type.
	/// </summary>
	public class DummyQueryReturnsValueTypeHandler : IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>
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
	}
}