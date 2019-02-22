-- in Lua, arrays start at index 1
local itemKey = KEYS[1]
local prefixForKeyToGroupKeyAssociation = ARGV[1]

local keyForItemKeyToGroupKeyAssociation = prefixForKeyToGroupKeyAssociation .. ":" .. itemKey

-- if the item belongs to a group...
if (redis.call('exists', keyForItemKeyToGroupKeyAssociation)) then
	
	-- delete the (groupKey -> item) association from the set named '[prefix]:groupKey'
	local associatedGroupKey = redis.call('get', keyForItemKeyToGroupKeyAssociation)
	redis.call('srem', associatedGroupKey, itemKey)

	-- delete the group if no items remain in the group (# is the length operator (http://www.lua.org/manual/5.1/manual.html#2.5.5))
	local remainingGroupMembers = redis.call('smembers', associatedGroupKey)
	if (#remainingGroupMembers == 0) then
		redis.call('del', associatedGroupKey)
	end

	-- delete the (item -> groupKey) association
	redis.call('del', keyForItemKeyToGroupKeyAssociation)
end

-- delete the item
redis.call('del', itemKey)