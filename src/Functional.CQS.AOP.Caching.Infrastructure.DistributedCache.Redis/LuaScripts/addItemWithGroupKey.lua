-- in Lua, arrays start at index 1
local itemKey = KEYS[1]
local groupKey = KEYS[2]
local serializedItem = ARGV[1]
local timeToLive = ARGV[2]
local prefixForKeyToGroupKeyAssociation = ARGV[3]
local prefixForGroupKey = ARGV[4]

local groupKeyToStore = prefixForGroupKey .. ":" .. groupKey

-- set item with expiry
redis.call('setex', itemKey, timeToLive, serializedItem)

-- set (item -> group) association with expiry
redis.call('setex', prefixForKeyToGroupKeyAssociation .. ":" .. itemKey, timeToLive, groupKeyToStore)

-- add itemKey to the set named '[prefix]:groupKey'; no expiry
redis.call('sadd', groupKeyToStore, itemKey)