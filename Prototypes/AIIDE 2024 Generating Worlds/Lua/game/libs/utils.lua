-- TODO:Put the coroutine and other util code right over here.
-- 1. One of the most important debug structures is visual outputs and
-- row and columns

-- File IO
-- WARNING: This file reader/writer require a hardcoded link to the file
-- I bet this is going to cause some issues. I just want to point it out for
-- future debugging.
function length_of_file(filename)
    local fh = assert(io.open(filename, "rb"))
    local len = assert(fh:seek("end"))
    fh:close()
    return len
end

function file_exists(path)
    local file = io.open(path, "rb")
    if file then file:close() end
    return file ~= nil
end

function seek(fh, ...)
    assert(fh:seek(...))
end

function readall(filename)
    local fh = assert(io.open(filename, "rb"))
    local contents = assert(fh:read(_VERSION <= "Lua 5.2" and "*a" or "a"))
    fh:close()
    return contents
end

function write(filename, contents)
    local fh = assert(io.open(filename, "wb"))
    fh.write(contents)
    fh:flush()
    fh:close()
end

function modify(filename, modify_func)
    local contents = readall(filename)
    contents = modify_func(contents)
    write(filename, contents)
end

if pcall(debug.getlocal, 4, 1) then
    print("In package")
else
    print("Main file")
    local path = "json.lua"
    print(file_exists(path))
end
