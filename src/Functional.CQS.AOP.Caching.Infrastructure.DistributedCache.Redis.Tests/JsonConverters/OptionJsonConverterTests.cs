using System;
using System.Collections.Generic;
using FluentAssertions;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.JsonConverters;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests.JsonConverters.Models;
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
			new OptionJsonConverter().CanConvert(Option.Some("value").GetType()).Should().BeTrue();
			new OptionJsonConverter().CanConvert(Option.Some(AppModel.Create()).GetType()).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSomeOptionOfInt()
		{
			const int SUCCESS_VALUE = 1337;
			var json = JsonConvert.SerializeObject(Option.Some(SUCCESS_VALUE), _jsonSerializerSettings);
			var fromJson = JsonConvert.DeserializeObject<Option<int>>(json, _jsonSerializerSettings);

			fromJson.Should().HaveExpectedValue(SUCCESS_VALUE);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSomeOptionOfString()
		{
			const string SUCCESS_VALUE = "value";
			var json = JsonConvert.SerializeObject(Option.Some(SUCCESS_VALUE), _jsonSerializerSettings);
			var fromJson = JsonConvert.DeserializeObject<Option<string>>(json, _jsonSerializerSettings);

			fromJson.Should().HaveExpectedValue(SUCCESS_VALUE);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSomeOptionOfSimplePOCO()
		{
			var obj = AppModel.Create();
			var json = JsonConvert.SerializeObject(Option.Some(obj), _jsonSerializerSettings);
			var fromJson = JsonConvert.DeserializeObject<Option<AppModel>>(json, _jsonSerializerSettings);

			fromJson.Should().HaveValue(x => x.IsLike(obj));
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSomeOptionOfComplexPOCO()
		{
			var obj = new AppModelWithVersion() { ApplicationInformation = AppModel.Create(), Version = new Version(4, 20, 0, 0) };
			var json = JsonConvert.SerializeObject(Option.Some(obj), _jsonSerializerSettings);
			var fromJson = JsonConvert.DeserializeObject<Option<AppModelWithVersion>>(json, _jsonSerializerSettings);

			fromJson.Should().HaveValue(x => x.IsLike(obj));
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