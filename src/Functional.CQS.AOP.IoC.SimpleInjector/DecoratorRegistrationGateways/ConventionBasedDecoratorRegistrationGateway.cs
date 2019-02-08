using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using SimpleInjector;
using Container = SimpleInjector.Container;

namespace Functional.CQS.AOP.IoC.SimpleInjector.DecoratorRegistrationGateways
{
	/// <summary>
	/// Gateway for applying decorators to Functional.CQS handler implementations that have been registered via convention.
	/// </summary>
	public sealed class ConventionBasedDecoratorRegistrationGateway
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConventionBasedDecoratorRegistrationGateway"/> class.
		/// </summary>
		/// <param name="container">The container containing prior registration of Functional.CQS handler implementations.</param>
		/// <param name="assemblies">The collection of assemblies to scan for performing further registrations.</param>
		/// <param name="lifestyle">The lifestyle that was specified when registering all Functional.CQS handler implementations.</param>
		public ConventionBasedDecoratorRegistrationGateway(Container container, IEnumerable<Assembly> assemblies, Lifestyle lifestyle)
		{
			Container = container ?? throw new ArgumentNullException(nameof(container));
			AssemblyCollection = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
			Lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
		}

		/// <summary>
		/// Gets the container holding prior registration of <see cref="IQueryHandler{TQuery, TResult}"/>.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Container Container { get; }

		/// <summary>
		/// Gets the collection of assemblies to scan for performing further registrations.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<Assembly> AssemblyCollection { get; }

		/// <summary>
		/// Gets the lifestyle that was specified for the original <see cref="IQueryHandler{TQuery, TResult}"/> registration.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Lifestyle Lifestyle { get; }

		#region ... hide object methods

		// ReSharper disable once BaseObjectEqualsIsObjectEquals
		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj) => base.Equals(obj);

		// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode() => base.GetHashCode();

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToString() => base.ToString();

		#endregion
	}
}