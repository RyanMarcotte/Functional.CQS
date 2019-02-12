-- in Lua, arrays start at index 1
local prefixForGroupKey = ARGV[1]

-- return all keys with the specified prefix
return redis.call('keys', prefixForGroupKey .. ':*')