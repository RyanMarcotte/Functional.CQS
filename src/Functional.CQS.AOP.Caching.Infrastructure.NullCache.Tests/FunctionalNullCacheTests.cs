using System;
using System.Threading.Tasks;
using FluentAssertions;
using Functional.Primitives.FluentAssertions;
using Xunit;

namespace Functional.CQS.AOP.Caching.Infrastructure.NullCache.Tests
{
	public class FunctionalNullCacheTests
	{
		[Fact]
		public void AlwaysExecutesTypedDelegate()
		{
			const string VALUE = "1337";
			int count = 0;

			string DataRetriever()
			{
				++count;
				return VALUE;
			}

			var sut = new FunctionalNullCache();
			var value1 = sut.Get("key", Option.None<string>(), DataRetriever, i => true, TimeSpan.FromSeconds(5));
			var value2 = sut.Get("key", Option.None<string>(), DataRetriever, i => true, TimeSpan.FromSeconds(5));

			value1.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);
			value2.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);

			count.Should().Be(2);
		}

		[Fact]
		public void AlwaysExecutesUntypedDelegate()
		{
			const string VALUE = "1337";
			int count = 0;

			object DataRetriever()
			{
				++count;
				return VALUE;
			}

			var sut = new FunctionalNullCache();
			var value1 = sut.Get("key", Option.None<string>(), typeof(string), DataRetriever, i => true, TimeSpan.FromSeconds(5));
			var value2 = sut.Get("key", Option.None<string>(), typeof(string), DataRetriever, i => true, TimeSpan.FromSeconds(5));

			value1.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);
			value2.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);

			count.Should().Be(2);
		}

		[Fact]
		public async Task AlwaysExecutesTypedAsyncDelegate()
		{
			const string VALUE = "1337";
			int count = 0;

			Task<string> DataRetriever()
			{
				++count;
				return Task.FromResult(VALUE);
			}

			var sut = new FunctionalNullCache();
			var value1 = await sut.GetAsync("key", Option.None<string>(), DataRetriever, i => true, TimeSpan.FromSeconds(5));
			var value2 = await sut.GetAsync("key", Option.None<string>(), DataRetriever, i => true, TimeSpan.FromSeconds(5));

			value1.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);
			value2.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);

			count.Should().Be(2);
		}

		[Fact]
		public async Task AlwaysExecutesUntypedAsyncDelegate()
		{
			const string VALUE = "1337";
			int count = 0;

			Task<object> DataRetriever()
			{
				++count;
				return Task.FromResult<object>(VALUE);
			}

			var sut = new FunctionalNullCache();
			var value1 = await sut.GetAsync("key", Option.None<string>(), typeof(string), DataRetriever, i => true, TimeSpan.FromSeconds(5));
			var value2 = await sut.GetAsync("key", Option.None<string>(), typeof(string), DataRetriever, i => true, TimeSpan.FromSeconds(5));

			value1.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);
			value2.Should().BeSuccessful().AndSuccessValue.Should().Be(VALUE);

			count.Should().Be(2);
		}
	}
}
