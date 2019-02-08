using Functional.CQS.AOP.IoC.SimpleInjector.Models;

// ReSharper disable once CheckNamespace
namespace SimpleInjector
{
	/// <summary>
	/// Extension methods for <see cref="DecoratorPredicateContext"/>.
	/// </summary>
	public static class DecoratorPredicateContextExtensions
	{
		/// <summary>
		/// Maps <see cref="DecoratorPredicateContext"/> to <see cref="ServiceAndImplementationType"/>.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static ServiceAndImplementationType ToServiceAndImplementationType(this DecoratorPredicateContext source) => new ServiceAndImplementationType(source.ServiceType, source.ImplementationType);
	}
}