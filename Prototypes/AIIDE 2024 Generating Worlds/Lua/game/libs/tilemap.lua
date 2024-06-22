-- TODO: Use asserts at some point for checking to see if the tilemap is the
-- correct size as the one that the game is using.
-- TODO: Make loader functions--really make this entire file into
-- util functions that are called in main.lua... Or have a function that
-- generates out all the information that we care about...
-- Maybe, just hear me out, maybe there should be a tile *generator*

local json = require('json')
local utils = require('game.libs.utils')

print(json, utils)

-- our tiles
tile = {}
-- TODO: change 3 to the number of tile images minus 1.
-- Figure out a way to make sure that the tile knows how many tiles that
-- there are. Really the way to do this is to have some XML (or JSON) that
-- contains the total number of tiles and that gets loaded in here.
for i = 0, 4 do
    tile[i] = love.graphics.newImage("tiles/tile" .. i .. ".png")
end

love.graphics.setNewFont(12)

-- TODO: Make sure these tilemap values are actually stored somewhere and
-- are useful later. I don't think that the placement of these variables are
-- correct, but hey, at least they are working as of now... This doesn't instill
-- confidence.
-- map variables
map_w = 20
map_h = 20
map_x = 0
map_y = 0
map_offset_x = 30
map_offset_y = 30
map_display_w = 14
map_display_h = 10
tile_w = 48
tile_h = 48

map = {
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 1, 0, 0, 2, 2, 2, 0, 3, 0, 3, 0, 1, 1, 1, 0, 0, 0, 0, 0 },
    { 0, 1, 0, 0, 2, 0, 2, 0, 3, 0, 3, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 1, 1, 0, 2, 2, 2, 0, 0, 3, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 },
    { 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 2, 2, 2, 0, 3, 3, 3, 0, 1, 1, 1, 0, 2, 0, 0, 0, 0, 0, 0 },
    { 0, 2, 0, 0, 0, 3, 0, 3, 0, 1, 0, 1, 0, 2, 0, 0, 0, 0, 0, 0 },
    { 0, 2, 0, 0, 0, 3, 0, 3, 0, 1, 0, 1, 0, 2, 0, 0, 0, 0, 0, 0 },
    { 0, 2, 2, 2, 0, 3, 3, 3, 0, 1, 1, 1, 0, 2, 2, 2, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
}

function draw_map()
    for y = 1, map_display_h do
        for x = 1, map_display_w do
            love.graphics.draw(
                tile[map[y + map_y][x + map_x]],
                (x * tile_w) + map_offset_x,
                (y * tile_h) + map_offset_y)
        end
    end
end

function love.keypressed(key, unicode)
    if key == 'up' then
        map_y = map_y - 1
        if map_y < 0 then map_y = 0; end
    end
    if key == 'down' then
        map_y = map_y + 1
        if map_y > map_h - map_display_h then map_y = map_h - map_display_h; end
    end

    if key == 'left' then
        map_x = math.max(map_x - 1, 0)
    end
    if key == 'right' then
        map_x = math.min(map_x + 1, map_w - map_display_w)
    end
end

-- TODO: This shouldn't be called unpackTilemap anymore, becuase that's not
-- what it's doing. As for what it's doing it's formatting and making the
-- tilemap for Love2D to understand... So I guess it's kind of like an unpack
function unpackTilemap(height, width, value)
    local formatted = string.format("height: %d, width %d, value: %s", height, width, value)
    local arr = {}
    local temp = ""
    local w, h = width, height

    -- TODO: Creating tile array... Really this should  make the tilemap...
    -- but alas I don't know the shape of the problem just yet to be able
    -- to make a perfect solution.
    for i = 1, #value, 1 do
        char = string.sub(value, i, i)
        if char == ',' then
            table.insert(arr, tonumber(temp))
            temp = ""
        elseif tonumber(char) then
            temp = temp .. char
        end
    end

    -- formatting the tile array into a tile map for the return.
    local map = {}
    local row_index = 0
    local temp = {}
    -- Printing out the tilearray to see what's going on...
    for _, value in ipairs(arr) do
        row_index = row_index + 1
        table.insert(temp, value)
        if row_index >= width then
            row_index = 0
            table.insert(map, temp)
            temp = {}
        end
    end
    printTilemap(map)
    print("unpacked tilemap!")
    return map
end

function printTilemap(data)
    local temp = ""
    if type(data) == "table" then
        for index, value in ipairs(data) do
            io.write(index .. " :")
            for _, int in ipairs(value) do
                io.write(int .. " ,")
            end
            io.write('\n')
        end
    end
end

function updateTilemap(tilemap)
    map = tilemap
end
