README.md
==========================================================================
Please note that this software is intended as a concurrent system.
<!-- NOTE: or is it... I don't know. Something to wonder. I think the game and client are coroutines.-->
## Random thoughts 

Ideally the structure of Lua Client code base should be broken up into two parts:

1. The networking (IE the server/client infrastructure)
2. The game (Love2D code)

There should be some main.lua file that runs both which then takes requests from the game and send it to the server.
I don't understand how this isn't any different than having a server running with the game, but idk.
TODO: Figure out better infrastructure for this project.

## Current TODO

- [ ] Uh, json.lua shouldn't be at the root folder... But here we are.
- [ ] At some point in the near future, refactor your code.
- [x] Once the game has started, the game should request a tilemap.
- [ ] WFC only generates an graph, which it then gives back to the server.
- [ ] Love game decodes the incoming data and generates Love2D understandable tilemap data structure, then gives to Love2D.
- [ ] Love2D takes the tilemap data and renders a tilemap from the tilemap data.

## Data flow

TODO: make a data flow diagram.
Right now the idea is outlined below:

- A user will start a new instance of a love2d game.

- When the game is started, the game will request a tilemap from the lua server that the game exists on.

- The lua server will send a request to a C# server via TCP/IP; 

- The C# server will first look at the request to see if it valid.
If it is valid then parse the request to WFC

- WFC runs until it can find a solution for the graph problem, or times out

- The graph is sent back to the lua server. This graph is sent over in some encoding that both systems can understand.

- The lua server constructs the tilemap json that is finally given to love.

- Love2D renders the pipeline. 

## Backlog

- [ ] use Tiled representation so it's more generic and anyone who is familiar with tiled can then interface with the tool that you make.

- [ ] Sixlabors is a graphics engine within C#? (Lots of questions about that.) Regardless though, removing Sixlabors seems to be the idea as WFC shouldn't generate images--just output graphs. TODO: Remove Sixlabors?

## Completed tasks

- [x] C# TCPlistener works and is sending dummy data to other servers.
- [x] Lua server up and running, which sends data to the C# server.
- [x] Love2D game starts.

- [x] Figure out how to display tiles in love2D.
