-- Dirty, unclean script for adding copyright headers.
-- Just ignore this.

do
	local function recursivelyDelete(item, depth)
		if love.filesystem.isDirectory(item) then
			for _, child in pairs(love.filesystem.getDirectoryItems(item)) do
				recursivelyDelete(item .. '/' .. child, depth + 1);
				love.filesystem.remove(item .. '/' .. child);
			end
		elseif love.filesystem.isFile(item) then
			love.filesystem.remove(item);
		end
		love.filesystem.remove(item)
	end

	if love.filesystem.isDirectory("out") then recursivelyDelete("out", 0) end
end
love.filesystem.createDirectory("out")

local isDir = love.filesystem.isDirectory
local getDir = love.filesystem.getDirectoryItems
local newDir = love.filesystem.createDirectory
local utf8 = require "utf8"

local function enum(path, cont, orig)
	if not cont then cont = {} end
	if not orig then orig = "^"..path.."/" end
	for k,v in pairs(getDir(path)) do
		local fullpath = path.."/"..v
		if isDir(fullpath) then
			enum(fullpath, cont, orig)
		else
			local ins = fullpath:gsub(orig,"")
			table.insert(cont, ins)
		end
	end
	return cont
end

local log = setmetatable({}, {
	__call = function(t, value)
		print(value)
		table.insert(t, tostring(value))
	end
})
for k,v in pairs(enum("Scripts")) do
	outpath = "out/"..v
	inpath = "Scripts/"..v

	local parent = string.match(outpath, "^(.*)[/\\].-$") or ""
	if not isDir(parent) then
		newDir(parent)
	end

	if v:find("!") then
	elseif v:find("%.cs$") then
		local content = love.filesystem.read(inpath)
		
		if not (content:find("</copyright>")) then
	
			love.filesystem.write(
				outpath,
				"//-----------------------------------------------------------------------\
// <copyright file=\"" .. v:gsub("^.*[/\\]", "") .. "\" company=\"" .. "COMPANYPLACEHOLDER" .."\">\
//     Copyright (c) Darius Kinstler. All rights reserved.\
// </copyright>\
//-----------------------------------------------------------------------\n\n" ..
				content)
		end
	end
end

love.filesystem.write("log.txt", table.concat(log, "\n"))

function love.run() end
