using SemanticComparison.Fluent;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.Tests.JsonConverters.Models
{
	internal class AppError
	{
		public string Message { get; set; }

		public static AppError Create() => new AppError()
		{
			Message = "the error message"
		};
	}

	internal static class AppErrorExtensions
	{
		public static void IsLike(this AppError source, AppError expected)
		{
			expected.AsSource().OfLikeness<AppError>().ShouldEqual(source);
		}
	}
}