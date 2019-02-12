using System.Text;
using Newtonsoft.Json;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	internal static class GenericExtensions
	{
		public static string ToJsonString<T>(this T source, JsonSerializerSettings serializerSettings)
		{
			byte[] s = source as byte[];
			if (s != null)
				return Encoding.UTF8.GetString(s);

			return JsonConvert.SerializeObject(source, serializerSettings);
		}
	}
}