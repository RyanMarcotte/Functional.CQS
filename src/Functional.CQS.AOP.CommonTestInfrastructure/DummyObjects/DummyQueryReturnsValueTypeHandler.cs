namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IQueryHandler{TQuery, TResult}"/> implementation.  Returns value type.
	/// </summary>
	public class DummyQueryReturnsValueTypeHandler : IQueryHandler<DummyQueryReturnsValueType, DummyQueryReturnsValueTypeResult>
	{
		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		public DummyQueryReturnsValueTypeResult Handle(DummyQueryReturnsValueType query)
		{
			return new DummyQueryReturnsValueTypeResult();
		}
	}
}