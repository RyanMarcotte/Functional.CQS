using System.Reflection;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="IQueryHandler{TQuery, TResult}"/> implementation.  Returns null.
	/// </summary>
	public class DummyQueryReturnsNullHandler : IQueryHandler<DummyQueryReturnsNull, DummyQueryReturnsNullResult>
	{
		private static readonly DummyQueryReturnsNull _query = new DummyQueryReturnsNull();
		private static readonly DummyQueryReturnsNullResult _result = null;

		/// <summary>
		/// Handle the query.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		public DummyQueryReturnsNullResult Handle(DummyQueryReturnsNull query)
		{
			return _result;
		}
	}
}