using System;
using System.Collections.Generic;
using System.Text;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching.Configuration
{
	/// <summary>
	/// Encapsulates configuration parameters used by Functional.CQS.AOP.IoC.SimpleInjector.Caching components.
	/// </summary>
	public class CachingModuleConfigurationParameters
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CachingModuleConfigurationParameters"/> class.
		/// </summary>
		/// <param name="queryResultCachingDecoratorEnabled">Indicates if the query result caching decorator is enabled.</param>
		public CachingModuleConfigurationParameters(bool queryResultCachingDecoratorEnabled)
		{
			QueryResultCachingDecoratorEnabled = queryResultCachingDecoratorEnabled;
		}

		/// <summary>
		/// Indicates if the query result caching decorator is enabled.
		/// </summary>
		public bool QueryResultCachingDecoratorEnabled { get; }
	}
}
