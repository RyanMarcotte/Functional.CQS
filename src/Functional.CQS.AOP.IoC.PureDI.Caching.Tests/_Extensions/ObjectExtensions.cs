using Functional.CQS.AOP.IoC.PureDI.Caching.Models;

// ReSharper disable once CheckNamespace
namespace Functional.CQS.AOP.IoC.PureDI.Caching.Tests
{
	internal static class ObjectExtensions
	{
		public static DataWrapper<T> ToDataWrapper<T>(this T obj) where T : class => new DataWrapper<T>(obj);
	}
}