using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata
{
	/// <summary>
	/// Provides an interface generating data related to sample CQS handler implementations provided for the unit test suite.
	/// </summary>
	public interface IProvideInformationAboutCQSHandlerDummyImplementation
	{
		/// <summary>
		/// Retrieves method metadata for the CQS handler type's Handle / HandleAsync method.
		/// </summary>
		/// <returns></returns>
		MethodInfo GetHandleMethodInfo();

		/// <summary>
		/// Retrieves the argument list used when calling the CQS handler type's Handle / HandleAsync method.
		/// </summary>
		/// <returns></returns>
		object[] GetArgumentsThatWillBePassedIntoDummyImplementationHandleMethod();

		/// <summary>
		/// Retrieves the return value of the CQS handler type.
		/// </summary>
		/// <returns></returns>
		object GetValueThatWillBeReturnedFromDummyImplementationHandleMethod();
	}

	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Extension methods for the <see cref="IProvideInformationAboutCQSHandlerDummyImplementation"/> interface.
	/// </summary>
	public static class ExtensionsFor_IProvideInformationAboutCQSHandlerDummyImplementation
	{
		/// <summary>
		/// Returns the underlying return value type of a CQS handler type's sample implementation.  If the handler returns Task&lt;T>, this method returns T."/>
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
		public static Type GetUnderlyingReturnValueType(this IProvideInformationAboutCQSHandlerDummyImplementation handlerType)
		{
			var methodInfo = handlerType.GetHandleMethodInfo();
			var returnValueType = methodInfo.ReturnType;
			if (!returnValueType.IsGenericType || (returnValueType.GetGenericTypeDefinition() != typeof(Task<>)))
				return returnValueType == typeof(Task) ? typeof(void) : returnValueType;

			return returnValueType.GetGenericArguments()[0];
		}
	}
}
