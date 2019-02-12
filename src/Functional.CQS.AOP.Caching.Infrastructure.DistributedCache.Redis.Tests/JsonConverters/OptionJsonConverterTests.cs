using System.Collections.Generic;
using FluentAssertions;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.JsonConverters;
using Functional.Primitives.FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests.JsonConverters
{
	public class OptionJsonConverterTests
	{
		private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
		{
			Converters = new List<JsonConverter> { new OptionJsonConverter() }
		};

		[Fact]
		public void ShouldBeAbleToConvertSomeOption()
		{
			new OptionJsonConverter().CanConvert(Option.Some(1337).GetType()).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSomeOption()
		{
			const int SUCCESS_VALUE = 1337;
			var json = JsonConvert.SerializeObject(Option.Some(SUCCESS_VALUE), _jsonSerializerSettings);
			var fromJson = JsonConvert.DeserializeObject<Option<int>>(json, _jsonSerializerSettings);

			fromJson.Should().HaveExpectedValue(SUCCESS_VALUE);
		}

		[Fact]
		public void ShouldBeAbleToConvertNoneOption()
		{
			new OptionJsonConverter().CanConvert(Option.None<int>().GetType()).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeNoneOption()
		{
			var json = JsonConvert.SerializeObject(Option.None<int>(), _jsonSerializerSettings);
			var fromJson = JsonConvert.DeserializeObject<Option<int>>(json, _jsonSerializerSettings);

			fromJson.Should().NotHaveValue();
		}
	}
}