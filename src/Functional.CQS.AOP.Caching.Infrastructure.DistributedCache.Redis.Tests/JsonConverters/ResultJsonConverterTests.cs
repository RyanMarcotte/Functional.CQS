using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.JsonConverters;
using Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests.JsonConverters.Models;
using Functional.Primitives.FluentAssertions;
using Jmansar.SemanticComparisonExtensions;
using Newtonsoft.Json;
using SemanticComparison;
using SemanticComparison.Fluent;
using Xunit;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests.JsonConverters
{
	public class ResultJsonConverterTests
	{
		static ResultJsonConverterTests()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
			{
				Converters = new List<JsonConverter> { new ResultJsonConverter() }
			};
		}

		[Fact]
		public void ShouldBeAbleToConvertSuccessfulResult()
		{
			new ResultJsonConverter().CanConvert(Result.Success<int, string>(1337).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Success<AppModel, Exception>(AppModel.Create()).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Success<AppModelWithVersion, Exception>(AppModelWithVersion.Create()).GetType()).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResult()
		{
			const int SUCCESS_VALUE = 1337;
			var json = JsonConvert.SerializeObject(Result.Success<int, string>(SUCCESS_VALUE));
			var fromJson = (Result<int, string>)JsonConvert.DeserializeObject(json, typeof(Result<int, string>));

			fromJson.Should().BeSuccessfulWithExpectedValue(SUCCESS_VALUE);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfEnumerableCollection()
		{
			var collection = Enumerable.Range(0, 10);
			var json = JsonConvert.SerializeObject(Result.Success<IEnumerable<int>, Exception>(collection));
			var fromJson = (Result<IEnumerable<int>, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<IEnumerable<int>, Exception>));

			fromJson.Should().BeSuccessful(x => x.SequenceEqual(collection).Should().BeTrue());
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfArray()
		{
			var array = Enumerable.Range(0, 10).ToArray();
			var json = JsonConvert.SerializeObject(Result.Success<int[], Exception>(array));
			var fromJson = (Result<int[], Exception>)JsonConvert.DeserializeObject(json, typeof(Result<int[], Exception>));

			fromJson.Should().BeSuccessful(x => x.SequenceEqual(array).Should().BeTrue());
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfSimplePOCO()
		{
			var obj = AppModel.Create();
			var json = JsonConvert.SerializeObject(Result.Success<AppModel, Exception>(obj));
			var fromJson = (Result<AppModel, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<AppModel, Exception>));

			fromJson.Should().BeSuccessful(x => x.IsLike(obj));
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfComplexPOCO()
		{
			var obj = AppModelWithVersion.Create();
			var json = JsonConvert.SerializeObject(Result.Success<AppModelWithVersion, Exception>(obj));
			var fromJson = (Result<AppModelWithVersion, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<AppModelWithVersion, Exception>));

			fromJson.Should().BeSuccessful(x => x.IsLike(obj));
		}

		[Fact]
		public void ShouldBeAbleToConvertFaultedResult()
		{
			new ResultJsonConverter().CanConvert(Result.Failure<int, string>("dead or alive, you're coming with me").GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Failure<AppModel, Exception>(new Exception("some error")).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Failure<AppModelWithVersion, Exception>(new Exception("ERROR!", new Exception("inner error"))).GetType()).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfEnumerableCollection()
		{
			var collection = Enumerable.Range(0, 10).Select(x => x.ToString());
			var json = JsonConvert.SerializeObject(Result.Failure<IEnumerable<int>, IEnumerable<string>>(collection));
			var fromJson = (Result<IEnumerable<int>, IEnumerable<string>>)JsonConvert.DeserializeObject(json, typeof(Result<IEnumerable<int>, IEnumerable<string>>));

			fromJson.Should().BeFaulted(x => x.SequenceEqual(collection).Should().BeTrue());
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfArray()
		{
			var array = Enumerable.Range(0, 10).Select(x => x.ToString()).ToArray();
			var json = JsonConvert.SerializeObject(Result.Failure<int[], string[]>(array));
			var fromJson = (Result<int[], string[]>)JsonConvert.DeserializeObject(json, typeof(Result<int[], string[]>));

			fromJson.Should().BeFaulted(x => x.SequenceEqual(array).Should().BeTrue());
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfSimplePOCO()
		{
			var obj = AppError.Create();
			var json = JsonConvert.SerializeObject(Result.Failure<AppModel, AppError>(obj));
			var fromJson = (Result<AppModel, AppError>)JsonConvert.DeserializeObject(json, typeof(Result<AppModel, AppError>));

			fromJson.Should().BeFaulted(x => x.IsLike(obj));
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfComplexPOCO()
		{
			var exception = new Exception("ERROR!", new Exception("inner error"));
			var json = JsonConvert.SerializeObject(Result.Failure<AppModelWithVersion, Exception>(exception));
			var fromJson = (Result<AppModelWithVersion, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<AppModelWithVersion, Exception>));

			fromJson.Should().BeFaulted(x =>
			{
				x.AsSource().OfLikeness<Exception>()
					.WithInnerLikeness(d => d.InnerException, s => s.InnerException, l => l.Without(ex => ex.Data))
					.Without(ex => ex.Data)
					.ShouldEqual(exception);
			});
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResult()
		{
			const string FAIL_VALUE = "dead or alive, you're coming with me";
			var json = JsonConvert.SerializeObject(Result.Failure<int, string>(FAIL_VALUE));
			var fromJson = JsonConvert.DeserializeObject<Result<int, string>>(json, new ResultJsonConverter());

			fromJson.Should().BeFaultedWithExpectedValue(FAIL_VALUE);
		}
	}
}
