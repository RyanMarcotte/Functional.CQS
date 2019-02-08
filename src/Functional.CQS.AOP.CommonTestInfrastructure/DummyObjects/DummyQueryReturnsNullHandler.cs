namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IQueryHandler{TQuery, TResult}"/> implementation.  Returns null.
	/// </summary>
	public class DummyQueryReturnsNullHandler : IQueryHandler<DummyQueryReturnsNull, DummyQueryReturnsNullResult>
	{
		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		public DummyQueryReturnsNullResult Handle(DummyQueryReturnsNull query)
		{
			return null;
		}
	}
}