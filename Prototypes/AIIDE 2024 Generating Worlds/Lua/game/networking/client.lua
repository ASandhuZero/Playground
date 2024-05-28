-- NOTE: At some point add in more diagnostic output to better understand
-- what is going on down below.
-- NOTE: Using JSON instead of XML, but I think that it should be changed to
-- XML at a later point.
-- NOTE: I don't think I fixed the blocking nature of this code... Got to
-- Figure that out eventually

local host, port = '127.0.0.1', 13000
local socket = require('socket')
local tcp = assert(socket.tcp())
local json = require('json')

-- TODO: Effectively we should be passing over the specsheet and also an entire
-- XML file as well, because WFC understands that.
-- I also think that the code that is requesting the data should be the one
-- that offers all the information... Sure. Read a book about servers at some
-- point.
-- TODO: Make some data structure that is a tilemap, as that structure
-- is what we can then verify with. As you will have many tilemaps coming in.
function client()
    tcp:connect(host, port)
    -- Note the newline below
    -- This should be turned into a function and then send specsheets.
    specsheet = {}
    specsheet["width"] = 30
    specsheet["height"] = 30
    -- TODO: You shouldn't be sending over just the spec as a dummy text.
    -- This should be grabbing data from a JSON file to send over to
    -- WFC
    specsheet["spec"] = "TEST"
    data = json.encode(specsheet)
    tcp:send(data)

    -- NOTE: Of course it's going to hang. There is a while true here...
    local s, status, partial
    while true do
        s, status, partial = tcp:receive()
        print(" This is the message received.")

        if status == 'closed' then break end
    end
    tcp:close()
    coroutine.yield(s or partial)
end

-- NOTE: This and send are the only functions that we should care about.
-- TODO: Uh, I think this is where all your problems really lie.
-- Figure out how to make the receive actually non-blocking...
function receive(connection)
    connection:settimeout(0) -- do not block
    local s, status, partial = connection:receive(2 ^ 10)
    if status == "timeout" then
        coroutine.yield(connection)
    end
    return s or partial, status
end

function send(message)
    local c = assert(socket.connect(host, port))
    local count = 0 -- counts number of bytes read
    -- local request = string.format(
    --     "GET %s HTTP/1.0\r\nhost: %s\r\n\r\n", file, host)
    c:send(message)
    print("sent request" .. request)
    while true do
        local s, status = receive(c)
        count = count + #s
        if status == "closed" then break end
    end
    c:close()
end

-- Checking if file is a library or has been executed.
if pcall(debug.getlocal, 4, 1) then
    print("In package")
else
    print("Main file")
    client()
end
