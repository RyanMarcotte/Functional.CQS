using System;

// ReSharper disable once CheckNamespace
namespace Newtonsoft.Json.Linq
{
	internal static class JTokenExtensions
	{
		public static T ToType<T>(this JToken source, string propertyName)
		{
			var jsonValue = source[propertyName];
			if (jsonValue.Type == JTokenType.Array)
				return JArray.Parse(jsonValue.ToString()).ToObject<T>();
			if (jsonValue.Type == JTokenType.Object)
				return JObject.Parse(jsonValue.ToString()).ToObject<T>();

			return jsonValue.Value<T>();
		}
	}
}