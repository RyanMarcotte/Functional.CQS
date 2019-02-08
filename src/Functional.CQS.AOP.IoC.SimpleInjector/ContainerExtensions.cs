using System.Reflection;
using Functional.CQS;
using Functional.CQS.AOP.IoC.SimpleInjector.DecoratorRegistrationGateways;

// ReSharper disable once CheckNamespace
namespace SimpleInjector
{
	/// <summary>
	/// Extension methods for <see cref="Container"/>.
	/// </summary>
	public static class ContainerExtensions
	{
		/// <summary>
		/// Registers all <see cref="IQueryHandler{TQuery,TResult}"/>, <see cref="IAsyncQueryHandler{TQuery, TResult}"/>, <see cref="ICommandHandler{TCommand, TError}"/>, <see cref="IAsyncCommandHandler{TCommand, TError}"/> 
		/// implementations defined across the specified <paramref name="assemblies"/>.  All handler implementations will be registered with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="lifestyle">The lifestyle.</param>
		/// <param name="assemblies">The assemblies containing Functional.CQS handler implementations.</param>
		public static ConventionBasedDecoratorRegistrationGateway RegisterAllFunctionalCQSHandlers(this Container container, Lifestyle lifestyle, params Assembly[] assemblies)
		{
			container.Register(typeof(IQueryHandler<,>), assemblies, lifestyle);
			container.Register(typeof(IAsyncQueryHandler<,>), assemblies, lifestyle);
			container.Register(typeof(ICommandHandler<,>), assemblies, lifestyle);
			container.Register(typeof(IAsyncCommandHandler<,>), assemblies, lifestyle);

			return new ConventionBasedDecoratorRegistrationGateway(container, assemblies, lifestyle);
		}
	}
}
