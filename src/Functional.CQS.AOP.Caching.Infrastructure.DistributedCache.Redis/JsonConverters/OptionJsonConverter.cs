﻿using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis.JsonConverters
{
	/// <summary>
	/// JSON converter for <see cref="Option{TValue}"/> type.
	/// </summary>
	public class OptionJsonConverter : JsonConverter
	{
		private const string HAS_VALUE_PROPERTY_NAME = "HasValue";
		private const string VALUE_PROPERTY_NAME = "Value";

		private static readonly MethodInfo _writeJsonMethod = typeof(OptionJsonConverter).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(WriteJson_Impl)) ?? throw new InvalidOperationException($"Unable to retrieve {nameof(WriteJson_Impl)} method info.");
		private static readonly MethodInfo _readJsonMethod = typeof(OptionJsonConverter).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(ReadJson_Impl)) ?? throw new InvalidOperationException($"Unable to retrieve {nameof(ReadJson_Impl)} method info.");

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var type = value.GetType();
			var optionType = type.GenericTypeArguments[0];

			var genericMethod = _writeJsonMethod.MakeGenericMethod(optionType);
			genericMethod.Invoke(null, new[] { writer, value });
		}

		private static void WriteJson_Impl<T>(JsonWriter writer, Option<T> value)
		{
			var jsonObject = new JObject();
			jsonObject.AddFirst(new JProperty(HAS_VALUE_PROPERTY_NAME, value.HasValue()));
			value.Apply(x => jsonObject.Add(new JProperty(VALUE_PROPERTY_NAME, JToken.FromObject(x))), () => { });

			jsonObject.WriteTo(writer);
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
			var optionType = objectType.GenericTypeArguments[0];
			var genericMethod = _readJsonMethod.MakeGenericMethod(optionType);
			return genericMethod.Invoke(null, new[] { reader });
		}

		private static Option<T> ReadJson_Impl<T>(JsonReader reader)
		{
			var item = JToken.Load(reader);
			var jsonObject = JToken.Parse(item.ToString());
			return Option.Create(jsonObject[HAS_VALUE_PROPERTY_NAME].Value<bool>(), () => jsonObject.ToType<T>(VALUE_PROPERTY_NAME));
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
			return genericTypeDefinition == typeof(Option<>);
		}
	}
}