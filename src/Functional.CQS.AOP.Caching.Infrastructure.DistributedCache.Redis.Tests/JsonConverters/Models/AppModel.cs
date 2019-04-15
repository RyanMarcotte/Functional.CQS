using System;
using SemanticComparison.Fluent;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests.JsonConverters.Models
{
	internal class AppModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public static AppModel Create() => new AppModel()
		{
			Id = Guid.NewGuid(),
			Name = Guid.NewGuid().ToString(),
			Description = Guid.NewGuid().ToString()
		};
	}

	internal static class AppModelExtensions
	{
		public static void IsLike(this AppModel source, AppModel expected)
		{
			expected.AsSource().OfLikeness<AppModel>().ShouldEqual(source);
		}
	}
}