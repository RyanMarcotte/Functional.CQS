using System;
using Newtonsoft.Json;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	/// <summary>
	/// Encapsulates Redis cache configuration options.
	/// </summary>
	[Obsolete("Use FunctionalRedisCacheConfiguration instead.")]
	public class FunctionalRedisCacheOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalRedisCacheOptions"/> class.
		/// </summary>
		/// <param name="connectionString">The Redis cache connection string.  Must adhere to supported formats as documented at https://github.com/ServiceStack/ServiceStack.Redis#redis-connection-strings. </param>
		/// <param name="jsonConverterCollection">The collection of custom JSON converters.</param>
		public FunctionalRedisCacheOptions(string connectionString, params JsonConverter[] jsonConverterCollection)
		{
			ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			JsonConverterCollection = jsonConverterCollection ?? throw new ArgumentNullException(nameof(jsonConverterCollection));
		}

		/// <summary>
		/// The Redis cache connection string.  Must adhere to supported formats as documented at https://github.com/ServiceStack/ServiceStack.Redis#redis-connection-strings.
		/// </summary>
		public string ConnectionString { get; }

		/// <summary>
		/// The collection of custom JSON converters.
		/// </summary>
		public JsonConverter[] JsonConverterCollection { get; }
	}
}