using System;
using FluentAssertions;
using SemanticComparison.Fluent;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests.JsonConverters.Models
{
	internal class AppModelWithVersion
	{
		public AppModel ApplicationInformation { get; set; }
		public Version Version { get; set; }

		public static AppModelWithVersion Create() => new AppModelWithVersion()
		{
			ApplicationInformation = AppModel.Create(),
			Version = new Version(4, 20, 0, 0)
		};
	}

	internal static class AppModelWithVersionExtensions
	{
		public static void IsLike(this AppModelWithVersion source, AppModelWithVersion expected)
		{
			source.ApplicationInformation.IsLike(expected.ApplicationInformation);
			source.Version.Should().Be(expected.Version);
		}
	}
}