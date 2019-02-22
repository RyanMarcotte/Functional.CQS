using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.JsonConverters
{
	/// <summary>
	/// JSON converter for <see cref="Result{TSuccess,TFailure}"/> type.
	/// </summary>
	public class ResultJsonConverter : JsonConverter
	{
		private const string IS_SUCCESSFUL_PROPERTY_NAME = "IsSuccessful";
		private const string VALUE_PROPERTY_NAME = "Value";

		private static readonly MethodInfo _writeJsonMethod = typeof(ResultJsonConverter).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(WriteJson_Impl)) ?? throw new InvalidOperationException($"Unable to retrieve {nameof(WriteJson_Impl)} method info.");
		private static readonly MethodInfo _readJsonMethod = typeof(ResultJsonConverter).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(ReadJson_Impl)) ?? throw new InvalidOperationException($"Unable to retrieve {nameof(ReadJson_Impl)} method info.");

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var type = value.GetType();
			var successType = type.GenericTypeArguments[0];
			var failType = type.GenericTypeArguments[1];

			var genericMethod = _writeJsonMethod.MakeGenericMethod(successType, failType);
			genericMethod.Invoke(null, new [] { writer, value });
		}

		private static void WriteJson_Impl<TSuccess, TFailure>(JsonWriter writer, Result<TSuccess, TFailure> value)
		{
			var o = new JObject();
			o.AddFirst(new JProperty(IS_SUCCESSFUL_PROPERTY_NAME, value.IsSuccess()));
			value.Apply(
				success => o.Add(new JProperty(VALUE_PROPERTY_NAME, success)),
				failure => o.Add(new JProperty(VALUE_PROPERTY_NAME, failure)));

			o.WriteTo(writer);
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var successType = objectType.GenericTypeArguments[0];
			var failType = objectType.GenericTypeArguments[1];

			var genericMethod = _readJsonMethod.MakeGenericMethod(successType, failType);
			return genericMethod.Invoke(null, new[] { reader });
		}

		private static Result<TSuccess, TFailure> ReadJson_Impl<TSuccess, TFailure>(JsonReader reader)
		{
			var item = JToken.Load(reader);
			var jsonObject = JObject.Parse(item.ToString());
			return Result.Create(jsonObject[IS_SUCCESSFUL_PROPERTY_NAME].Value<bool>(),
				() => jsonObject[VALUE_PROPERTY_NAME].Value<TSuccess>(),
				() => jsonObject[VALUE_PROPERTY_NAME].Value<TFailure>());
		}

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
		public override bool CanConvert(Type objectType)
		{
			if (!objectType.IsConstructedGenericType)
				return false;
					
			var genericTypeDefinition = objectType.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(Result<,>);
		}
	}
}