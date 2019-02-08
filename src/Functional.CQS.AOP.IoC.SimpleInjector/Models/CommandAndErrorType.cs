using System;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Models
{
	/// <summary>
	/// Encapsulates the command and error types associated with <see cref="ICommandHandler{TCommand, TError}"/> and <see cref="IAsyncCommandHandler{TCommand, TError}"/>.
	/// </summary>
	public struct CommandAndErrorType : IEquatable<CommandAndErrorType>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandAndErrorType"/> class.
		/// </summary>
		/// <param name="commandType">The command type.</param>
		/// <param name="errorType">The error type.</param>
		public CommandAndErrorType(Type commandType, Type errorType)
		{
			CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
			ErrorType = errorType ?? throw new ArgumentNullException(nameof(errorType));
		}

		/// <summary>
		/// Gets the command type.
		/// </summary>
		public Type CommandType { get; }

		/// <summary>
		/// Gets the error type.
		/// </summary>
		public Type ErrorType { get; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns></returns>
		public bool Equals(CommandAndErrorType other) => (CommandType == other.CommandType) && (ErrorType == other.ErrorType);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			return (obj.GetType() == this.GetType()) && Equals((CommandAndErrorType)obj);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((CommandType != null ? CommandType.GetHashCode() : 0) * 397) ^ (ErrorType != null ? ErrorType.GetHashCode() : 0);
			}
		}
	}
}