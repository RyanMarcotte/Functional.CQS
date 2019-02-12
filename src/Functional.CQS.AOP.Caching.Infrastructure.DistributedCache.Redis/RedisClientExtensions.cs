using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis
{
	internal static class RedisClientExtensions
	{
		private const string DIRECTORY_NAME = "LuaScripts";
		private const string KEY_TO_GROUP_KEY_PREFIX = "keyToGroupKey";
		private const string GROUP_KEY_PREFIX = "groupKey";

		private static readonly Guid _addItemWithGroupKeyScriptID = Guid.NewGuid();
		private static readonly Guid _removeItemScriptID = Guid.NewGuid();
		private static readonly Guid _removeItemGroupScriptID = Guid.NewGuid();
		private static readonly Guid _countKeyToGroupKeyAssociationItems = Guid.NewGuid();
		private static readonly Guid _countGroupKeyItems = Guid.NewGuid();

		private static readonly Assembly _assembly = typeof(FunctionalRedisCache).Assembly; 
		private static readonly IReadOnlyDictionary<Guid, string> _scriptLookup = new Dictionary<Guid, string>
		{
			{ _addItemWithGroupKeyScriptID, _assembly.LoadScript("addItemWithGroupKey.lua").Match(value => value, ex => throw ex) },
			{ _removeItemScriptID, _assembly.LoadScript("removeItem.lua").Match(value => value, ex => throw ex) },
			{ _removeItemGroupScriptID, _assembly.LoadScript("removeItemGroup.lua").Match(value => value, ex => throw ex) },
			{ _countKeyToGroupKeyAssociationItems, _assembly.LoadScript("countKeyToGroupKeyAssociationItems.lua").Match(value => value, ex => throw ex) },
			{ _countGroupKeyItems, _assembly.LoadScript("countGroupKeyItems.lua").Match(value => value, ex => throw ex) }
		};

		public static Result<bool, Exception> SetSafely<T>(this IRedisClient client, string key, T item, TimeSpan timeToLive, JsonSerializerSettings serializerSettings) => Result.Try(() => client.Set(key, item.ToJsonString(serializerSettings), timeToLive));

		public static Result<RedisText, Exception> SetWithGroupKeySafely<T>(this IRedisClient client, string key, string groupKey, T item, TimeSpan timeToLive, JsonSerializerSettings serializerSettings)
		{
			var keys = new[] { key, groupKey };
			var args = new[] { item.ToJsonString(serializerSettings), ((int) timeToLive.TotalSeconds).ToString(CultureInfo.InvariantCulture), KEY_TO_GROUP_KEY_PREFIX, GROUP_KEY_PREFIX };
			return client.ExecCachedLua(_scriptLookup[_addItemWithGroupKeyScriptID], sha1 => client.ExecuteLuaShaSafely(sha1, keys, args));
		}

		public static Result<RedisText, Exception> RemoveSafely(this IRedisClient client, string key)
		{
			var keys = new[] { key, };
			var args = new[] { KEY_TO_GROUP_KEY_PREFIX };
			return client.ExecCachedLua(_scriptLookup[_removeItemScriptID], sha1 => client.ExecuteLuaShaSafely(sha1, keys, args));
		}

		public static Result<RedisText, Exception> RemoveGroupSafely(this IRedisClient client, string groupKey)
		{
			var keys = new[] { groupKey };
			var args = new[] { KEY_TO_GROUP_KEY_PREFIX, GROUP_KEY_PREFIX };
			return client.ExecCachedLua(_scriptLookup[_removeItemGroupScriptID], sha1 => client.ExecuteLuaShaSafely(sha1, keys, args));
		}

		public static Result<int, Exception> CountKeyToGroupKeyAssociationItems(this IRedisClient client)
		{
			return client.ExecCachedLua(_scriptLookup[_countKeyToGroupKeyAssociationItems], sha1 => client.ExecuteLuaShaAsListSafely(sha1, new string[] { }, new[] { KEY_TO_GROUP_KEY_PREFIX }).Select(results => results.Count()));
		}

		public static Result<int, Exception> CountGroupKeySetItems(this IRedisClient client)
		{
			return client.ExecCachedLua(_scriptLookup[_countGroupKeyItems], sha1 => client.ExecuteLuaShaAsListSafely(sha1, new string[] { }, new[] { GROUP_KEY_PREFIX })).Select(results => results.Count());
		}

		private static Result<string, Exception> LoadScript(this Assembly assembly, string scriptName)
		{
			return Result.Try(() =>
			{
				string resourceName = $"{typeof(FunctionalRedisCache).Namespace}.{DIRECTORY_NAME}.{scriptName}";
				using (var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new NullReferenceException($"Unable to load Lua script file with resource name '{resourceName}'"))
				{
					using (var streamReader = new StreamReader(stream, Encoding.UTF8))
						return streamReader.ReadToEnd();
				}
			});
		}

		private static Result<RedisText, Exception> ExecuteLuaShaSafely(this IRedisClient client, string sha1, string[] keys, string[] args) => Result.Try(() => client.ExecLuaSha(sha1, keys, args));
		private static Result<IEnumerable<string>, Exception> ExecuteLuaShaAsListSafely(this IRedisClient client, string sha1, string[] keys, string[] args) => Result.Try(() => client.ExecLuaShaAsList(sha1, keys, args).AsEnumerable());
	}
}