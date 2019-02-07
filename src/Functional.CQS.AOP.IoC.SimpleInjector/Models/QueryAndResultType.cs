using System;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Models
{
	/// <summary>
	/// Encapsulates the query and result types associated with <see cref="IQueryHandler{TQuery, TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/>.
	/// </summary>
	public struct QueryAndResultType : IEquatable<QueryAndResultType>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryAndResultType"/> class.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		public QueryAndResultType(Type queryType, Type resultType)
		{
			QueryType = queryType ?? throw new ArgumentNullException(nameof(queryType));
			ResultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
		}

		/// <summary>
		/// Gets the query type.
		/// </summary>
		public Type QueryType { get; }

		/// <summary>
		/// Gets the result type.
		/// </summary>
		public Type ResultType { get; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns></returns>
		public bool Equals(QueryAndResultType other) => (QueryType == other.QueryType) && (ResultType == other.ResultType);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			return (obj.GetType() == this.GetType()) && Equals((QueryAndResultType)obj);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((QueryType != null ? QueryType.GetHashCode() : 0) * 397) ^ (ResultType != null ? ResultType.GetHashCode() : 0);
			}
		}
	}
}