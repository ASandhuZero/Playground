-- TODO: Please clean up the code base and make a flow chart of what is
-- going on.
-- Then once that is done, push up your code.
local thread    -- Our thread object.
local timer = 0 -- A timer used to animate our circle.
require('networking.client')
require('tilemap')

local tasks = {}

function stop()
    table.insert(tasks, 'stop')
end

function runloop()
    while true do
        local nextCo = table.remove(tasks, 1)
        if nextCo == 'stop' then
            break
        else
            local status, value = coroutine.resume(nextCo)
            if value then
                tilemap = unpackTilemap(map_h, map_w, value)
                -- TODO: Maybe we shouldn't just have a global that we are
                -- updating everywhere... Maybe it should be something else?
                updateTilemap(tilemap)
            end
        end
    end
end

function love.load()
    tasks = {}
    sendRequest()
end

function love.update(dt)
    timer = timer + dt
    stop()
    runloop()
end

function love.draw()
    draw_map()
    -- Get the info channel and pop the next message from it.
    -- We smoothly animate a circle to show that the thread isn't blocking our main thread.
    -- love.graphics.circle('line', 100 + math.sin(timer) * 20, 100 + math.cos(timer) * 20, 20)
end

function love.keypressed(k)
    if k == 'escape' then
        love.event.quit()
    end
    if k == 's' then
        sendRequest()
    end
end

function sendRequest()
    local co = coroutine.create(function()
        client()
    end)
    table.insert(tasks, co)
end
