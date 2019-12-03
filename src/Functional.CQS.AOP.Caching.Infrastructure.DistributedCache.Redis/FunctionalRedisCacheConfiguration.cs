using System;
using Newtonsoft.Json;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	/// <summary>
	/// Encapsulates Redis cache configuration.
	/// </summary>
	public class FunctionalRedisCacheConfiguration
	{
		private const string LOCALHOST = "localhost";

		private FunctionalRedisCacheConfiguration(string hostURL, int portNumber, params JsonConverter[] jsonConverterCollection)
		{
			HostURL = hostURL ?? throw new ArgumentNullException(nameof(hostURL));
			PortNumber = portNumber;
			JsonConverterCollection = jsonConverterCollection ?? throw new ArgumentNullException(nameof(jsonConverterCollection));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalRedisCacheConfiguration"/> class.
		/// </summary>
		/// <param name="jsonConverterCollection">The collection of custom JSON converters.</param>
		public static FunctionalRedisCacheConfiguration ForLocalHostPort6379(params JsonConverter[] jsonConverterCollection)
			=> new FunctionalRedisCacheConfiguration(LOCALHOST, 6379, jsonConverterCollection);

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalRedisCacheConfiguration"/> class.
		/// </summary>
		/// <param name="portNumber">The port number.</param>
		/// <param name="jsonConverterCollection">The collection of custom JSON converters.</param>
		public static FunctionalRedisCacheConfiguration ForLocalHost(int portNumber, params JsonConverter[] jsonConverterCollection)
			=> new FunctionalRedisCacheConfiguration(LOCALHOST, portNumber, jsonConverterCollection);

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalRedisCacheConfiguration"/> class.
		/// </summary>
		/// <param name="hostURL">The host URL.</param>
		/// <param name="portNumber">The port number.</param>
		/// <param name="jsonConverterCollection">The collection of custom JSON converters.</param>
		public static FunctionalRedisCacheConfiguration ForRemoteHost(string hostURL, int portNumber, params JsonConverter[] jsonConverterCollection)
			=> new FunctionalRedisCacheConfiguration(hostURL, portNumber, jsonConverterCollection);

		/// <summary>
		/// The host URL for the Redis cache.
		/// </summary>
		public string HostURL { get; }

		/// <summary>
		/// The port number.
		/// </summary>
		public int PortNumber { get; }

		/// <summary>
		/// The collection of custom JSON converters.
		/// </summary>
		public JsonConverter[] JsonConverterCollection { get; }
	}
}