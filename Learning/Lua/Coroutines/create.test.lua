-- co = coroutine.create(function()
--     for i = 1, 10 do
--         print("co", i)
--         coroutine.yield()
--     end
-- end)
--
-- print(co)
-- print(coroutine.status(co))
-- for i = 1, 20 do
--     coroutine.resume(co)
--     print(coroutine.status(co))
-- end

co = coroutine.create(function(a,b,c)
    print("co", a, b, c+2)
end)

coroutine.resume(co, 1,2,3)


co = coroutine.create(function(x)
    print('co1', x)
    print('co2', coroutine.yield())
end)

coroutine.resume(co, "hi")
coroutine.resume(co, "YEP")

co = coroutine.create(function()
    return 6, 7
end)

print(coroutine.resume(co))



