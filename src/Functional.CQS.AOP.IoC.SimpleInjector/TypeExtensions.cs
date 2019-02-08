using System;
using System.Collections.Generic;
using System.Linq;
using Functional.CQS.AOP.IoC.SimpleInjector.Models;

// Ambiguous reference in cref attribute (for Option.Some)
#pragma warning disable CS0419
#pragma warning disable 1574

namespace Functional.CQS.AOP.IoC.SimpleInjector
{
	/// <summary>
	/// Extension methods for <see cref="Type"/>.
	/// </summary>
    public static class TypeExtensions
    {
		/// <summary>
		/// Analyzes a type and returns a closed generic interface (i.e. typeof(IFoo&lt;int&gt;)) if that type implements the open generic interface (i.e. typeof(IFoo&lt;&gt;)).
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="openGenericInterfaceType">The open generic interface type (i.e. typeof(IFoo&lt;&gt;), typeof(IBar&lt;,&gt;)).</param>
		/// <returns></returns>
		public static Option<Type> GetClosedGenericInterfaceTypeFromOpenGenericInterfaceType(this Type type, Type openGenericInterfaceType)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceTypes(new[] { openGenericInterfaceType });
		}

		/// <summary>
		/// Analyzes a type and returns a closed generic interface (i.e. typeof(IFoo&lt;int&gt;)) if that type implements any open generic interface specified (i.e. typeof(IFoo&lt;&gt;)).
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="openGenericInterfaceTypes">The open generic interface types (i.e. typeof(IFoo&lt;&gt;), typeof(IBar&lt;,&gt;)).</param>
		/// <returns></returns>
		public static Option<Type> GetClosedGenericInterfaceTypeFromOpenGenericInterfaceTypes(this Type type, IEnumerable<Type> openGenericInterfaceTypes)
		{
			return Option.FromNullable(type.IsInterface
				? type.IsConstructedGenericType && openGenericInterfaceTypes.Contains(type.GetGenericTypeDefinition()) ? type : null
				: type.GetInterfaces().Where(x => x.IsConstructedGenericType).FirstOrDefault(x => openGenericInterfaceTypes.Contains(x.GetGenericTypeDefinition())));
		}

		/// <summary>
		/// Analyzes a <see cref="IQueryHandler{TQuery, TResult}"/> or <see cref="IAsyncQueryHandler{TQuery, TResult}"/> type and returns <see cref="Option.Some{T}"/> of TQuery, TResult type; otherwise, return <see cref="Option.None{T}"/>.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <returns></returns>
		public static Option<QueryAndResultType> GetGenericParametersForQueryHandlerType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceTypes(_queryHandlerTypeCollection).Select(queryHandlerInterface =>
			{
				var queryType = queryHandlerInterface.GenericTypeArguments[0];
				var resultType = queryHandlerInterface.GenericTypeArguments[1];
				return new QueryAndResultType(queryType, resultType);
			});
		}

		/// <summary>
		/// Analyzes a <see cref="ICommandHandler{TCommand, TError}"/> or <see cref="IAsyncCommandHandler{TCommand, TErro}"/> type and returns <see cref="Option.Some{T}"/> of TCommand, TError type; otherwise, return <see cref="Option.None{T}"/>.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <returns></returns>
		public static Option<CommandAndErrorType> GetGenericParametersForCommandHandlerType(this Type type)
		{
			return type.GetClosedGenericInterfaceTypeFromOpenGenericInterfaceTypes(_commandHandlerTypeCollection).Select(resultCommandHandlerInterface =>
			{
				var commandType = resultCommandHandlerInterface.GenericTypeArguments[0];
				var errorType = resultCommandHandlerInterface.GenericTypeArguments[1];
				return new CommandAndErrorType(commandType, errorType);
			});
		}

		private static readonly IEnumerable<Type> _queryHandlerTypeCollection = new HashSet<Type>()
		{
			typeof(IQueryHandler<,>),
			typeof(IAsyncQueryHandler<,>)
		};

		private static readonly IEnumerable<Type> _commandHandlerTypeCollection = new HashSet<Type>()
		{
			typeof(ICommandHandler<,>),
			typeof(IAsyncCommandHandler<,>)
		};
	}
}

// Ambiguous reference in cref attribute (for Option.Some)
#pragma warning restore 1574
#pragma warning restore CS0419
