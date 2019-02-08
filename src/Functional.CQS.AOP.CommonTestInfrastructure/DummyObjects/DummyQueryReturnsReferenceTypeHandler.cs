namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IQueryHandler{TQuery, TResult}"/> implementation.  Returns reference type.
	/// </summary>
	public class DummyQueryReturnsReferenceTypeHandler : IQueryHandler<DummyQueryReturnsReferenceType, DummyQueryReturnsReferenceTypeResult>
	{
		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		public DummyQueryReturnsReferenceTypeResult Handle(DummyQueryReturnsReferenceType query)
		{
			return new DummyQueryReturnsReferenceTypeResult();
		}
	}
}