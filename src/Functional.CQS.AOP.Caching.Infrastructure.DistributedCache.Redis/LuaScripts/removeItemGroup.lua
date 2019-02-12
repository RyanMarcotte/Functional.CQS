-- in Lua, arrays start at index 1
local groupKey = KEYS[1]
local prefixForKeyToGroupKeyAssociation = ARGV[1]
local prefixForGroupKey = ARGV[2]

local groupKeyStored = prefixForGroupKey .. ":" .. groupKey

-- retrieve all members of the group and delete them
local itemsToDelete = redis.call('smembers', groupKeyStored)
for _, key in pairs(itemsToDelete) do 
	redis.call('del', key)
	redis.call('del', prefixForKeyToGroupKeyAssociation .. ":" .. key)
end

-- delete the group
redis.call('del', groupKeyStored)