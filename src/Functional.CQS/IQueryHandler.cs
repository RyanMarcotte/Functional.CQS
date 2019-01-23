using System;

namespace Functional.CQS
{
	/// <summary>
	/// Interface for synchronous query handlers.
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
