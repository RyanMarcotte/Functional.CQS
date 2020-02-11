using FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests
{
	public class FunctionalRedisCacheConfigurationTests
	{
		[Theory]
		[InlineData("abc123", "www.github.com:8000,password=abc123")]
		[InlineData("", "www.github.com:8000")]
		[InlineData(null, "www.github.com:8000")]
		public void ToConnectionStringShouldReturnAppropriateConnectionStringWithPassword(string password, string expectedResult)
		{
			var functionalRedisCacheConfiguration =
				FunctionalRedisCacheConfiguration.ForRemoteHost("www.github.com", 8000, password);
			
			var result = functionalRedisCacheConfiguration.ToConnectionString();
			
			result.Should().Be(expectedResult);
		}
	}
}
