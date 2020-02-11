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
				Converters = new List<JsonConverter> { new OptionJsonConverter(), new ResultJsonConverter() }
			};
		}

		[Fact]
		public void ShouldBeAbleToConvertSuccessfulResult()
		{
			new ResultJsonConverter().CanConvert(Result.Success<int, string>(1337).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Success<AppModel, Exception>(AppModel.Create()).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Success<AppModelWithVersion, Exception>(AppModelWithVersion.Create()).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Success<Option<AppModel>, Exception>(Option.Some(AppModel.Create())).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Success<Option<AppModel>, Exception>(Option.None<AppModel>()).GetType()).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResult()
		{
			const int SUCCESS_VALUE = 1337;
			var json = JsonConvert.SerializeObject(Result.Success<int, string>(SUCCESS_VALUE));
			var fromJson = (Result<int, string>)JsonConvert.DeserializeObject(json, typeof(Result<int, string>));

			fromJson.Should().BeSuccessful().AndSuccessValue.Should().Be(SUCCESS_VALUE);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfEnumerableCollection()
		{
			var collection = Enumerable.Range(0, 10);
			var json = JsonConvert.SerializeObject(Result.Success<IEnumerable<int>, Exception>(collection));
			var fromJson = (Result<IEnumerable<int>, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<IEnumerable<int>, Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.SequenceEqual(collection).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfArray()
		{
			var array = Enumerable.Range(0, 10).ToArray();
			var json = JsonConvert.SerializeObject(Result.Success<int[], Exception>(array));
			var fromJson = (Result<int[], Exception>)JsonConvert.DeserializeObject(json, typeof(Result<int[], Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.SequenceEqual(array).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfSimplePOCO()
		{
			var obj = AppModel.Create();
			var json = JsonConvert.SerializeObject(Result.Success<AppModel, Exception>(obj));
			var fromJson = (Result<AppModel, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<AppModel, Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.IsLike(obj);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfComplexPOCO()
		{
			var obj = AppModelWithVersion.Create();
			var json = JsonConvert.SerializeObject(Result.Success<AppModelWithVersion, Exception>(obj));
			var fromJson = (Result<AppModelWithVersion, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<AppModelWithVersion, Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.IsLike(obj);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfOptionOfSomeSimplePOCO()
		{
			var obj = AppModel.Create();
			var json = JsonConvert.SerializeObject(Result.Success<Option<AppModel>, Exception>(Option.Some(obj)));
			var fromJson = (Result<Option<AppModel>, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<Option<AppModel>, Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.Should().HaveValue().AndValue.IsLike(obj);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfOptionOfNoSimplePOCO()
		{
			var json = JsonConvert.SerializeObject(Result.Success<Option<AppModel>, Exception>(Option.None<AppModel>()));
			var fromJson = (Result<Option<AppModel>, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<Option<AppModel>, Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.Should().NotHaveValue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfOptionOfSomeComplexPOCO()
		{
			var obj = AppModelWithVersion.Create();
			var json = JsonConvert.SerializeObject(Result.Success<Option<AppModelWithVersion>, Exception>(Option.Some(obj)));
			var fromJson = (Result<Option<AppModelWithVersion>, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<Option<AppModelWithVersion>, Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.Should().HaveValue().AndValue.IsLike(obj);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeSuccessfulResultOfOptionOfNoComplexPOCO()
		{
			var json = JsonConvert.SerializeObject(Result.Success<Option<AppModelWithVersion>, Exception>(Option.None<AppModelWithVersion>()));
			var fromJson = (Result<Option<AppModelWithVersion>, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<Option<AppModelWithVersion>, Exception>));

			fromJson.Should().BeSuccessful().AndSuccessValue.Should().NotHaveValue();
		}

		[Fact]
		public void ShouldBeAbleToConvertFaultedResult()
		{
			new ResultJsonConverter().CanConvert(Result.Failure<int, string>("dead or alive, you're coming with me").GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Failure<AppModel, Exception>(new Exception("some error")).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Failure<AppModelWithVersion, Exception>(new Exception("ERROR!", new Exception("inner error"))).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Failure<Option<AppModel>, Exception>(new Exception("some error")).GetType()).Should().BeTrue();
			new ResultJsonConverter().CanConvert(Result.Failure<Option<AppModelWithVersion>, Exception>(new Exception("ERROR!", new Exception("inner error"))).GetType()).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResult()
		{
			const string FAIL_VALUE = "dead or alive, you're coming with me";
			var json = JsonConvert.SerializeObject(Result.Failure<int, string>(FAIL_VALUE));
			var fromJson = JsonConvert.DeserializeObject<Result<int, string>>(json, new ResultJsonConverter());

			fromJson.Should().BeFaulted().AndFailureValue.Should().Be(FAIL_VALUE);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfEnumerableCollection()
		{
			var collection = Enumerable.Range(0, 10).Select(x => x.ToString());
			var json = JsonConvert.SerializeObject(Result.Failure<IEnumerable<int>, IEnumerable<string>>(collection));
			var fromJson = (Result<IEnumerable<int>, IEnumerable<string>>)JsonConvert.DeserializeObject(json, typeof(Result<IEnumerable<int>, IEnumerable<string>>));

			fromJson.Should().BeFaulted().AndFailureValue.SequenceEqual(collection).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfArray()
		{
			var array = Enumerable.Range(0, 10).Select(x => x.ToString()).ToArray();
			var json = JsonConvert.SerializeObject(Result.Failure<int[], string[]>(array));
			var fromJson = (Result<int[], string[]>)JsonConvert.DeserializeObject(json, typeof(Result<int[], string[]>));

			fromJson.Should().BeFaulted().AndFailureValue.SequenceEqual(array).Should().BeTrue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfSimplePOCO()
		{
			var obj = AppError.Create();
			var json = JsonConvert.SerializeObject(Result.Failure<AppModel, AppError>(obj));
			var fromJson = (Result<AppModel, AppError>)JsonConvert.DeserializeObject(json, typeof(Result<AppModel, AppError>));

			fromJson.Should().BeFaulted().AndFailureValue.IsLike(obj);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfComplexPOCO()
		{
			var exception = new Exception("ERROR!", new Exception("inner error"));
			var json = JsonConvert.SerializeObject(Result.Failure<AppModelWithVersion, Exception>(exception));
			var fromJson = (Result<AppModelWithVersion, Exception>)JsonConvert.DeserializeObject(json, typeof(Result<AppModelWithVersion, Exception>));

			fromJson
				.Should()
				.BeFaulted()
				.AndFailureValue
				.AsSource().OfLikeness<Exception>()
				.WithInnerLikeness(d => d.InnerException, s => s.InnerException, l => l.Without(ex => ex.Data))
				.Without(ex => ex.Data)
				.ShouldEqual(exception);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfOptionOfSomeSimplePOCO()
		{
			var obj = AppError.Create();
			var json = JsonConvert.SerializeObject(Result.Failure<AppModel, Option<AppError>>(Option.Some(obj)));
			var fromJson = (Result<AppModel, Option<AppError>>)JsonConvert.DeserializeObject(json, typeof(Result<AppModel, Option<AppError>>));

			fromJson.Should().BeFaulted().AndFailureValue.Should().HaveValue().AndValue.IsLike(obj);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfOptionOfNoSimplePOCO()
		{
			var json = JsonConvert.SerializeObject(Result.Failure<AppModel, Option<AppError>>(Option.None<AppError>()));
			var fromJson = (Result<AppModel, Option<AppError>>)JsonConvert.DeserializeObject(json, typeof(Result<AppModel, Option<AppError>>));

			fromJson.Should().BeFaulted().AndFailureValue.Should().NotHaveValue();
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfOptionOfSomeComplexPOCO()
		{
			var exception = new Exception("ERROR!", new Exception("inner error"));
			var json = JsonConvert.SerializeObject(Result.Failure<AppModelWithVersion, Option<Exception>>(Option.Some(exception)));
			var fromJson = (Result<AppModelWithVersion, Option<Exception>>)JsonConvert.DeserializeObject(json, typeof(Result<AppModelWithVersion, Option<Exception>>));

			fromJson
				.Should()
				.BeFaulted()
				.AndFailureValue
				.Should()
				.HaveValue()
				.AndValue
				.AsSource().OfLikeness<Exception>()
				.WithInnerLikeness(d => d.InnerException, s => s.InnerException, l => l.Without(ex => ex.Data))
				.Without(ex => ex.Data)
				.ShouldEqual(exception);
		}

		[Fact]
		public void ShouldBeAbleToSerializeAndDeserializeFaultedResultOfOptionOfNoComplexPOCO()
		{
			var json = JsonConvert.SerializeObject(Result.Failure<AppModelWithVersion, Option<Exception>>(Option.None<Exception>()));
			var fromJson = (Result<AppModelWithVersion, Option<Exception>>)JsonConvert.DeserializeObject(json, typeof(Result<AppModelWithVersion, Option<Exception>>));

			fromJson.Should().BeFaulted().AndFailureValue.Should().NotHaveValue();
		}
	}
}
