using System;

namespace Functional.CQS
{
	/// <summary>
	/// Interface for synchronous query handlers.  Intended to encapsulate application logic that returns a result and is free of side effects (i.e. no observable changes made to system).
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	public interface IQueryHandler<in TQuery, out TResult>
		where TQuery : IQueryParameters<TResult>
	{
		/// <summary>
		/// Execute the query.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <returns></returns>
		TResult Handle(TQuery parameters);
	}
}
