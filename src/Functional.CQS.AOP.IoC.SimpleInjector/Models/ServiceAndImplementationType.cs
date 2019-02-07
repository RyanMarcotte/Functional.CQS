using System;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Models
{
	/// <summary>
	/// Encapsulates a service type and its associated implementation type.
	/// </summary>
	public struct ServiceAndImplementationType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceAndImplementationType"/> struct.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="implementationType"></param>
		public ServiceAndImplementationType(Type serviceType, Type implementationType)
		{
			ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
		}

		/// <summary>
		/// Gets the service type.
		/// </summary>
		public Type ServiceType { get; }

		/// <summary>
		/// Gets the implementation type.
		/// </summary>
		public Type ImplementationType { get; }
	}
}