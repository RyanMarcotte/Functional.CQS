using System;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	/// <summary>
	/// Encapsulates Redis cache configuration options.
	/// </summary>
	public class FunctionalRedisCacheOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalRedisCacheOptions"/> class.
		/// </summary>
		/// <param name="connectionString">The Redis cache connection string.  Must adhere to supported formats as documented at https://github.com/ServiceStack/ServiceStack.Redis#redis-connection-strings. </param>
		public FunctionalRedisCacheOptions(string connectionString)
		{
			ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		}

		/// <summary>
		/// The Redis cache connection string.  Must adhere to supported formats as documented at https://github.com/ServiceStack/ServiceStack.Redis#redis-connection-strings.
		/// </summary>
		public string ConnectionString { get; }
	}
}