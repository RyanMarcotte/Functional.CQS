using System;
using System.Collections.Generic;
using System.Text;

namespace Functional.CQS.AOP.IoC.PureDI.Caching.Models
{
	/// <summary>
	/// Encapsulates a reference-type value.
	/// </summary>
	/// <typeparam name="T">The type.</typeparam>
	/// <remarks>
	/// Used to ensure that cache implementations never store NULL, and to ensure that all types assignable to <typeparamref name="T"/> are treated as <typeparamref name="T"/>.
	/// </remarks>
	public class DataWrapper<T>
		where T : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataWrapper{T}"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		public DataWrapper(T data)
		{
			// we WANT to allow nulls here
			Data = data;
		}

		/// <summary>
		/// Gets the data.
		/// </summary>
		public T Data { get; }
	}
}
